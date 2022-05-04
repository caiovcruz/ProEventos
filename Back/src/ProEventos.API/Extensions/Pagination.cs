using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ProEventos.API.Models;

namespace ProEventos.API.Extensions
{
    public static class Pagination
    {
        public static void AddPagination(this HttpResponse response,
                                        int currentPage,
                                        int itemsPerPage,
                                        int totalItems,
                                        int totalPages)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(new PaginationHeader(currentPage,
                                                                                            itemsPerPage,
                                                                                            totalItems,
                                                                                            totalPages), options));

            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}