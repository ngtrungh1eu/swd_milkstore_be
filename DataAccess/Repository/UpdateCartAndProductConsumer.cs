using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DataAccess.Data;
using DataAccess.EntityModel;

namespace DataAccess.Repository
{
    public class UpdateCartAndProductsConsumer
    {
        private readonly DataContext _context;

        public UpdateCartAndProductsConsumer(DataContext context)
        {
            _context = context;
        }

        public void Start()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "update_cart_and_products_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var updateMessage = JsonConvert.DeserializeObject<UpdateCartAndProductsMessage>(message);

                if (updateMessage != null)
                {
                    var cart = await _context.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync(c => c.CartId == updateMessage.CartId);

                    if (cart != null)
                    {
                        // Update product quantities
                        foreach (var item in cart.CartItems)
                        {
                            var product = item.Product;

                            if (product != null)
                            {
                                if (product.isPreOrder)
                                {
                                    product.PreOrderAmount -= item.Quantity;
                                }
                                else
                                {
                                    product.Quantity -= item.Quantity;
                                }

                                _context.Products.Update(product);
                            }
                        }

                        // Clear cart items
                        _context.CartItems.RemoveRange(cart.CartItems);
                        cart.TotalItem = 0;
                        cart.TotalPrice = 0;
                        _context.Carts.Update(cart);

                        await _context.SaveChangesAsync();
                    }
                }
            };

            channel.BasicConsume(queue: "update_cart_and_products_queue", autoAck: true, consumer: consumer);

            Console.WriteLine("Consumer started. Press [enter] to exit.");
            Console.ReadLine();
        }
    }

}
