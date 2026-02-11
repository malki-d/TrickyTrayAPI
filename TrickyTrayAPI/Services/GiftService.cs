using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Drawing;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;
using WebApi.Data;
using System.IO;
using System.Text;

namespace TrickyTrayAPI.Services
{
    public class GiftService : IGiftService

    {
        private readonly IGiftRepository _giftrepository;
        private readonly ICartItemRepository _cartitemRepository;

        private readonly ILogger<GiftService> _logger;

        public GiftService(ICartItemRepository cartitemRepository, IGiftRepository giftrepository, ILogger<GiftService> logger)
        {
            _giftrepository = giftrepository;
            _logger = logger;
            _cartitemRepository = cartitemRepository;

        }

        public async Task<IEnumerable<GetGiftDTO>> GetAllAsync()
        {
            try
            {
                var donors = await _giftrepository.GetAllAsync();
                _logger.LogInformation("get gifts");

                return donors.Select(x => new GetGiftDTO() { Id = x.Id, Name = x.Name, Description = x.Description, Category = x.Category.Name, DonorName = x.Donor.Name, ImgUrl = x.ImgUrl });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "cant get gifts");
                return null;
            }
        }

        public async Task<GetGiftDTO?> GetByIdAsync(int id)
        {
            try
            {
                var gift = await _giftrepository.GetByIdAsync(id);
                _logger.LogInformation("get gift by id " + id);

                return new GetGiftDTO { Id = gift.Id, Name = gift.Name, Description = gift.Description, Category = gift.Category.Name, DonorName = gift.Donor.Name, ImgUrl = gift.ImgUrl };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant get gift by id " + id);
                throw;
            }

        }

        public async Task<GetGiftDTO> AddAsync(CreateGiftDTO giftDto)
        {
            try
            {
                // 1. יצירת שם ייחודי לקובץ ושמירתו
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(giftDto.ImageFile.FileName);
                string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string filePath = Path.Combine(wwwrootPath, "images", fileName);

                // וודא שהתיקייה קיימת
                if (!Directory.Exists(Path.Combine(wwwrootPath, "images")))
                    Directory.CreateDirectory(Path.Combine(wwwrootPath, "images"));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await giftDto.ImageFile.CopyToAsync(stream);
                }

                // 2. שליחה ל-Repository עם הנתיב היחסי
                string relativePath = "/images/" + fileName;
                var newGift = await _giftrepository.AddAsync(giftDto, relativePath);

                _logger.LogInformation("Gift created with image: " + newGift.Id);

                return new GetGiftDTO
                {
                    Id = newGift.Id,
                    Name = newGift.Name,
                    Description = newGift.Description,
                    Category = newGift.Category.Name,
                    DonorName = newGift.Donor.Name,
                    ImgUrl = newGift.ImgUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add gift with image");
                throw;
            }
        }

        public async Task<GetGiftDTO> UpdateAsync(UpdateGiftDTO giftDto, int id)
        {
            try
            {
                // 1. שליפת הנתונים הקיימים כדי לדעת מה היה נתיב התמונה הישנה
                var existingGift = await _giftrepository.GetByIdAsync(id);
                if (existingGift == null) throw new Exception("Gift not found");

                string finalImgUrl = existingGift.ImgUrl;

                // 2. בדיקה אם המשתמש העלה קובץ תמונה חדש
                if (giftDto.ImageFile != null)
                {
                    // שמירת הקובץ החדש
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(giftDto.ImageFile.FileName);
                    string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string filePath = Path.Combine(wwwrootPath, "images", fileName);

                    if (!Directory.Exists(Path.Combine(wwwrootPath, "images")))
                        Directory.CreateDirectory(Path.Combine(wwwrootPath, "images"));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await giftDto.ImageFile.CopyToAsync(stream);
                    }

                    // מחיקת הקובץ הישן מהשרת (אופציונלי אך מומלץ)
                    string oldFilePath = Path.Combine(wwwrootPath, existingGift.ImgUrl.TrimStart('/'));
                    if (File.Exists(oldFilePath)) File.Delete(oldFilePath);

                    finalImgUrl = "/images/" + fileName;
                }

                // 3. עדכון ה-DTO עם הנתיב הנכון (החדש או הקיים)
                giftDto.ImgUrl = finalImgUrl;

                var updateGift = await _giftrepository.UpdateAsync(giftDto, id);

                return new GetGiftDTO
                {
                    Id = updateGift.Id,
                    Name = updateGift.Name,
                    Description = updateGift.Description,
                    Category = updateGift.Category.Name,
                    DonorName = updateGift.Donor.Name,
                    ImgUrl = updateGift.ImgUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant update gift " + id);
                throw;
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {

                var isSucceed = await _giftrepository.DeleteAsync(id);
                if (isSucceed)
                {
                    _logger.LogInformation("delete gift " + id);
                }
                return isSucceed;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant delete gift " + id);
                return false;
                ;
            }

        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var isSucceed = await _giftrepository.ExistsAsync(id);
                _logger.LogInformation("check Exists gift " + id);
                return isSucceed;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant check Exists gift " + id);
                return false;

            }
        }
        public async Task<IEnumerable<GetGiftDTO>> SearchAsync(string? giftName, string? donorName, int? purchaserCount)
        {
            var gifts = await _giftrepository.SearchAsync(giftName, donorName, purchaserCount);
            return gifts.Select(g => new GetGiftDTO
            {
                Id = g.Id,
                Name = g.Name,
                DonorName = g.Donor.Name,
                Description = g.Description,
                Category = g.Category.Name,
                ImgUrl = g.ImgUrl
            });
        }
        public async Task<IEnumerable<GetGiftDTO>> GetSortedAsync(bool sortByName, bool sortByCategory)
        {
            var gifts = await _giftrepository.GetSortedAsync(sortByName, sortByCategory);
            return gifts.Select(g => new GetGiftDTO
            {
                Id = g.Id,
                Name = g.Name,
                DonorName = g.Donor.Name,
                Description = g.Description,
                Category = g.Category.Name,
                ImgUrl = g.ImgUrl
            });
        }
        public async Task<IEnumerable<GetGiftDTO>> GetByCategoryAsync(int categoryId)
        {
            var gifts = await _giftrepository.GetByCategoryAsync(categoryId);
            return gifts.Select(g => new GetGiftDTO
            {
                Id = g.Id,
                Name = g.Name,
                DonorName = g.Donor.Name,
                Description = g.Description,
                Category = g.Category.Name,
                ImgUrl = g.ImgUrl
                // הוסף שדות נוספים לפי הצורך
            });
        }

        public async Task<IEnumerable<GetGiftDTO>> GetSortedGiftsAsync(string sortBy)
        {
            try
            {
                var donors = await _giftrepository.GetSortedGiftsAsync(sortBy);
                _logger.LogInformation("get GetSortedGiftsAsync");

                return donors.Select(x => new GetGiftDTO() { Id = x.Id, Name = x.Name, Description = x.Description, Category = x.Category.Name, DonorName = x.Donor.Name, ImgUrl = x.ImgUrl });

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, "cant GetSortedGiftsAsync");
                return null;
            }
        }

        public async Task<IEnumerable<GiftWinnerReportDTO>> GetGiftWinnersReportAsync()
        {
            return await _giftrepository.GetGiftWinnersReportAsync();
        }

        public async Task<byte[]> ExportWinnersReportCsvAsync()
        {
            var report = await _giftrepository.GetGiftWinnersReportAsync();
            var sb = new StringBuilder();
            sb.AppendLine("GiftId,GiftName,WinnerId,WinnerName,WinnerEmail");
            foreach (var r in report)
            {
                var line = $"{r.GiftId},\"{r.GiftName.Replace("\"", "\"\"")}\",{r.WinnerId ?? 0},\"{(r.WinnerName ?? string.Empty).Replace("\"", "\"\"")}\",\"{(r.WinnerEmail ?? string.Empty).Replace("\"", "\"\"")}\"";
                sb.AppendLine(line);
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> ExportWinnersReportExcelAsync()
        {
            var report = await _giftrepository.GetGiftWinnersReportAsync();
            // Create a minimal SpreadsheetML (XML) for Excel compatibility
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\">\n<Worksheet ss:Name=\"Winners\">\n<Table>");
            sb.AppendLine("<Row>");
            sb.AppendLine("<Cell><Data>GiftId</Data></Cell>");
            sb.AppendLine("<Cell><Data>GiftName</Data></Cell>");
            sb.AppendLine("<Cell><Data>WinnerId</Data></Cell>");
            sb.AppendLine("<Cell><Data>WinnerName</Data></Cell>");
            sb.AppendLine("<Cell><Data>WinnerEmail</Data></Cell>");
            sb.AppendLine("</Row>");
            foreach (var r in report)
            {
                sb.AppendLine("<Row>");
                sb.AppendLine($"<Cell><Data>{r.GiftId}</Data></Cell>");
                sb.AppendLine($"<Cell><Data>{System.Security.SecurityElement.Escape(r.GiftName ?? string.Empty)}</Data></Cell>");
                sb.AppendLine($"<Cell><Data>{r.WinnerId ?? 0}</Data></Cell>");
                sb.AppendLine($"<Cell><Data>{System.Security.SecurityElement.Escape(r.WinnerName ?? string.Empty)}</Data></Cell>");
                sb.AppendLine($"<Cell><Data>{System.Security.SecurityElement.Escape(r.WinnerEmail ?? string.Empty)}</Data></Cell>");
                sb.AppendLine("</Row>");
            }
            sb.AppendLine("</Table>\n</Worksheet>\n</Workbook>");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<IEnumerable<GetGiftWithWinnerDTO>> RandomWinners()
        {
            // 1. קודם כל מבצעים את ההגרלה במסד הנתונים
            await _giftrepository.RunAllRandomWinnersAsync();

            // 2. מנקים את ה-Tracker כדי לראות את השינויים בשליפה הבאה
            _giftrepository.ClearTracker();

            // 3. שולפים את המתנות עם הזוכים החדשים
            var updatedGifts = await _giftrepository.GetAllAsync();

            // 4. ורק עכשיו מוחקים את פריטי העגלה
            var cartitems = await _cartitemRepository.GetAllAsync();
            foreach (var item in cartitems)
            {
                await _cartitemRepository.DeleteAsync(item.Id);
            }

            // 5. מחזירים את התוצאה למשתמש
            return updatedGifts.Select(g => new GetGiftWithWinnerDTO
            {
                Name = g.Name,
                Description = g.Description,
                Category = g.Category?.Name,
                ImgUrl = g.ImgUrl,
                WinnerName = g.Winner != null ? $"{g.Winner.FirstName} {g.Winner.LastName}" : "No Winner",
                WinnerEmail = g.Winner != null ? g.Winner.Email : "No Winner"
            });
        }
        public async Task<bool> RandomWinnerAsync(int giftId)
        {
            Random rnd = new Random();
            var gifts = await _giftrepository.GetByIdAsync(giftId);
            if (gifts == null)
            {
                _logger.LogInformation("cannot find gift " + giftId);
                return false;
            }
            var users = gifts.Users.ToList();
            if (users.Count == 0)
            {
                _logger.LogInformation("no users for gift " + giftId);
                return false;
            }
            var winnerIndex = rnd.Next(users.Count);
            var winnerId = users[winnerIndex].Id;
            return await _giftrepository.UpdateWinnerAsync(giftId, winnerId, true);

        }
    }
}
