﻿using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace DataAcess
{
    public class UserDAO
    {
        private readonly LibraryDbContext _context;
        public UserDAO(LibraryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Status);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> CreateAsync(User user)
        {
            // Kiểm tra trùng username (không phân biệt hoa thường)
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == user.Username.ToLower()))
                throw new System.Exception("Username da ton tai!");
            // Hash password nếu có nhập
            if (!string.IsNullOrEmpty(user.PasswordHash))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            else
                throw new System.Exception("Password khong duoc de trong!");
            user.CreatedAt = System.DateTime.Now;
            if (string.IsNullOrEmpty(user.ImageUrl))
                user.ImageUrl = "default-user.png";
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(int id, User user)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return null;
            existing.FullName = user.FullName;
            existing.Email = user.Email;
            existing.Phone = user.Phone;
            existing.Role = user.Role;
            existing.Status = user.Status;
            existing.ImageUrl = string.IsNullOrEmpty(user.ImageUrl) ? "default-user.png" : user.ImageUrl;
            // Nếu nhập password mới thì hash lại
            if (!string.IsNullOrEmpty(user.PasswordHash))
                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> SearchAsync(string keyword)
        {
            return await _context.Users
                .Where(u => u.FullName.Contains(keyword) || u.Username.Contains(keyword))
                .ToListAsync();
        }
        public async Task<UserRegisterModel> Register(UserRegisterModel model)
        {
            // Kiểm tra trùng username (không phân biệt hoa thường)
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower()))
                throw new System.Exception("Username da ton tai!");
            // Hash password
            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
            var user = new User
            {
                Username = model.Username,
                PasswordHash = model.PasswordHash,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Role = "Reader", // Mặc định là User
                Status = true, // Mặc định là active
                CreatedAt = System.DateTime.Now,
                ImageUrl = "default-user.png"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return model;
        }
    }
}