using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IVoteRepository
    {
        Task<Vote> GetVoteById(int id);
        Task<bool> AddVote(int userId, Vote vote);
        Task<bool> UpdateVote(int userId, Vote vote);
    }
    public class VoteRepository : IVoteRepository
    {
        private readonly DataContext _context;
        public VoteRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddVote(int userId, Vote vote)
        {
            var existingVote = await _context.Votes.AnyAsync(v => (v.UserId == userId) && (v.BlogId == vote.BlogId));

            if(existingVote)
            {
                return false;
            }

            Vote _newVote = new Vote()
            {
                UserId = userId,
                BlogId = vote.BlogId,
                VoteType = vote.VoteType,
            };

            _context.Votes.Add(_newVote);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<Vote> GetVoteById(int id)
        {
            return await _context.Votes.FirstOrDefaultAsync(v => v.VoteId == id);
        }

        public async Task<bool> UpdateVote(int userId, Vote vote)
        {
            var existingVote = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.VoteId == vote.VoteId);

            existingVote.VoteType = vote.VoteType;

            _context.Votes.Update(existingVote);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
