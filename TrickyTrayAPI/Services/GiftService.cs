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
{    public class GiftService : IGiftService

    {
        private readonly IGiftRepository _giftrepository;
        private readonly ICartItemRepository _cartitemRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<GiftService> _logger;

        public GiftService(ICartItemRepository cartitemRepository, IGiftRepository giftrepository, IEmailService emailService, ILogger<GiftService> logger)
        {
            _giftrepository = giftrepository;
            _logger = logger;
            _cartitemRepository = cartitemRepository;
            _emailService = emailService;
        }

        public async Task<IEnumerable<GetGiftDTO>> GetAllAsync()
        {
            try
            {
                var gifts = await _giftrepository.GetAllAsync();
                _logger.LogInformation("get gifts");

                return gifts.Select(x => new GetGiftDTO() { TicketsSold = x.purchaseItems.Count , Id = x.Id, Name = x.Name, Description = x.Description, Category = x.Category.Name, DonorName = x.Donor.Name, ImgUrl = x.ImgUrl ,WinnerName=x.Winner?.FirstName+" "+ x.Winner?.LastName,WinnerEmail=x.Winner?.Email,CategoryId=x.CategoryId,DonorId=x.DonorId,CanDelete=x.purchaseItems.Count==0});

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant get gifts");
                // ערך ברירת מחדל – רשימה ריקה
                return Enumerable.Empty<GetGiftDTO>();
            }
        }

        public async Task<GetGiftDTO?> GetByIdAsync(int id)
        {
            try
            {
                var gift = await _giftrepository.GetByIdAsync(id);
                _logger.LogInformation("get gift by id {GiftId}", id);

                if (gift == null)
                {
                    _logger.LogWarning("Gift with id {GiftId} not found in GetByIdAsync", id);
                    return null;
                }

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
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Validation error while adding gift (missing related entity)");
                throw;
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
                if (existingGift == null)
                {
                    throw new KeyNotFoundException($"Gift with id {id} not found");
                }

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
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Gift not found while updating {GiftId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cant update gift {GiftId}", id);
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
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "database error while deleting gift {GiftId}", id);
                return false;
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
            try
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
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "invalid arguments in SearchAsync");
                return Enumerable.Empty<GetGiftDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in SearchAsync");
                throw;
            }
        }
        public async Task<IEnumerable<GetGiftDTO>> GetSortedAsync(bool sortByName, bool sortByCategory)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in GetSortedAsync");
                throw;
            }
        }
        public async Task<IEnumerable<GetGiftDTO>> GetByCategoryAsync(int categoryId)
        {
            try
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in GetByCategoryAsync for category {CategoryId}", categoryId);
                throw;
            }
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
                _logger.LogError(ex, "cant GetSortedGiftsAsync");
                return Enumerable.Empty<GetGiftDTO>();
            }
        }

        public async Task<IEnumerable<GiftWinnerReportDTO>> GetGiftWinnersReportAsync()
        {
            try
            {
                return await _giftrepository.GetGiftWinnersReportAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in GetGiftWinnersReportAsync");
                throw;
            }
        }

        public async Task<byte[]> ExportWinnersReportCsvAsync()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in ExportWinnersReportCsvAsync");
                throw;
            }
        }

        public async Task<byte[]> ExportWinnersReportExcelAsync()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in ExportWinnersReportExcelAsync");
                throw;
            }
        }        public async Task<IEnumerable<GetGiftWithWinnerDTO>> RandomWinners()
        {
            try
            {
                _logger.LogInformation("🎲 Starting RandomWinners process...");
                
                // 1. קודם כל מבצעים את ההגרלה במסד הנתונים - מקבלים רשימת מזהי מתנות שזכו כעת
                var newWinnerGiftIds = await _giftrepository.RunAllRandomWinnersAsync();
                
                _logger.LogInformation($"📊 {newWinnerGiftIds.Count} new winners selected in this draw");

                // 2. מנקים את ה-Tracker כדי לראות את השינויים בשליפה הבאה
                _giftrepository.ClearTracker();

                // 3. שולפים את המתנות עם הזוכים החדשים
                var updatedGifts = await _giftrepository.GetAllAsync();
                
                // 4. שליחת מיילים רק לזוכים החדשים (מההגרלה הנוכחית)
                var newWinnerGifts = updatedGifts.Where(g => newWinnerGiftIds.Contains(g.Id) && g.Winner != null).ToList();
                _logger.LogInformation($"📧 Sending emails to {newWinnerGifts.Count} new winners...");

                foreach (var gift in newWinnerGifts)
                {
                    var winnerName = $"{gift.Winner.FirstName} {gift.Winner.LastName}";
                    _logger.LogInformation($"   → Sending to {winnerName} ({gift.Winner.Email}) for gift: {gift.Name}");
                    await _emailService.SendWinnerEmailAsync(gift.Winner.Email, winnerName, gift.Name);
                }

                // 5. ורק עכשיו מוחקים את פריטי העגלה
                var cartitems = await _cartitemRepository.GetAllAsync();
                foreach (var item in cartitems)
                {
                    await _cartitemRepository.DeleteAsync(item.Id);
                }

                _logger.LogInformation("✅ RandomWinners process completed successfully");

                // 6. מחזירים את התוצאה למשתמש
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in RandomWinners");
                throw;
            }
        }
        public async Task<bool> RandomWinnerAsync(int giftId)
        {
            try
            {
                Random rnd = new Random();
                var gifts = await _giftrepository.GetByIdAsync(giftId);
                if (gifts == null)
                {
                    _logger.LogInformation("cannot find gift {GiftId}", giftId);
                    return false;
                }
                var users = gifts.purchaseItems.ToList();
                if (users.Count == 0)
                {
                    _logger.LogInformation("no users for gift {GiftId}", giftId);
                    return false;
                }
                var winnerIndex = rnd.Next(users.Count);
                var winnerId = users[winnerIndex].Id;
                return await _giftrepository.UpdateWinnerAsync(giftId, winnerId, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error in RandomWinnerAsync for gift {GiftId}", giftId);
                return false;
            }
        }
    }
}
