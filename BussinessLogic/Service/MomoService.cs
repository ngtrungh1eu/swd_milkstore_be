using BussinessLogic.DTO.Momo;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface IMomoService
    {
        Task<CreatePaymentResponse> CreatePaymentAsync(OrderInfoModel model);
        ExecuteResponse PaymentExecuteAsync(IQueryCollection collection);
    }
    public class MomoService: IMomoService
    {
        private readonly  IOptions<MomoOptionModel> _options;
        private readonly ILogger<MomoService> _logger;
        public MomoService(IOptions<MomoOptionModel> options, ILogger<MomoService> logger)
        {
            _options = options;
            _logger = logger;
        }
        public async Task<CreatePaymentResponse> CreatePaymentAsync(OrderInfoModel model)
        {
            model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInfo = $"Khách hàng: {model.FullName}. Nội dung: {model.OrderInfo}. CartId: {model.cartId}";

            var rawData =
                $"partnerCode={_options.Value.PartnerCode}" +
                $"&accessKey={_options.Value.AccessKey}" +
                $"&requestId={model.OrderId}" +
                $"&amount={model.Amount}" +
                $"&orderId={model.OrderId}" +
                $"&orderInfo={model.OrderInfo}" +
                $"&returnUrl={_options.Value.ReturnUrl}" +
                $"&notifyUrl={_options.Value.NotifyUrl}" +
                $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = model.OrderId,
                amount = model.Amount.ToString(),
                orderInfo = model.OrderInfo,
                requestId = model.OrderId,
                extraData = "",
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            _logger.LogInformation($"Response content: {response.Content}");
            return JsonConvert.DeserializeObject<CreatePaymentResponse>(response.Content);

        }
        public ExecuteResponse PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.FirstOrDefault(s => s.Key == "amount").Value.FirstOrDefault();
            var orderInfo = collection.FirstOrDefault(s => s.Key == "orderInfo").Value.FirstOrDefault();
            var orderId = collection.FirstOrDefault(s => s.Key == "orderId").Value.FirstOrDefault();
            var cartId = collection.FirstOrDefault(s => s.Key == "cartId").Value.FirstOrDefault();

            if (amount == null || orderInfo == null || orderId == null)
            {
                throw new ArgumentException("Missing required query parameters");
            }


            return new ExecuteResponse()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                CartId = cartId,
            };
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }


    }
}
