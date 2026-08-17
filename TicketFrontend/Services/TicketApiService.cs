using System.Net.Http.Json;
using TicketFrontend.Models;

namespace TicketFrontend.Services;

public class TicketApiService
{
    private readonly HttpClient _httpClient;

    public TicketApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ============================================================
    // LOGIN
    // ============================================================

    public async Task<LoginResponseViewModel?> LoginAsync(
        LoginViewModel request)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "api/auth/login",
                request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<LoginResponseViewModel>();
    }

    // ============================================================
    // Get all tickets
    // ============================================================

    public async Task<List<TicketViewModel>> GetTicketsAsync()
    {
        var tickets =
            await _httpClient
                .GetFromJsonAsync<
                    List<TicketViewModel>>(
                    "api/tickets");

        return tickets ??
            new List<TicketViewModel>();
    }

    // ============================================================
    // Get single ticket
    // ============================================================

    public async Task<TicketViewModel?> GetTicketAsync(
        int id)
    {
        return await _httpClient
            .GetFromJsonAsync<TicketViewModel>(
                $"api/tickets/{id}");
    }

    // ============================================================
    // Get categories
    // ============================================================

    public async Task<List<CategoryViewModel>>
        GetCategoriesAsync()
    {
        var categories =
            await _httpClient
                .GetFromJsonAsync<
                    List<CategoryViewModel>>(
                    "api/categories");

        return categories ??
            new List<CategoryViewModel>();
    }

    // ============================================================
    // Get priorities
    // ============================================================

    public async Task<List<PriorityViewModel>>
        GetPrioritiesAsync()
    {
        var priorities =
            await _httpClient
                .GetFromJsonAsync<
                    List<PriorityViewModel>>(
                    "api/priorities");

        return priorities ??
            new List<PriorityViewModel>();
    }

    // ============================================================
    // Get statuses
    // ============================================================

    public async Task<List<StatusViewModel>>
        GetStatusesAsync()
    {
        var statuses =
            await _httpClient
                .GetFromJsonAsync<
                    List<StatusViewModel>>(
                    "api/statuses");

        return statuses ??
            new List<StatusViewModel>();
    }

    // ============================================================
    // Get users
    // ============================================================

    public async Task<List<UserViewModel>>
        GetUsersAsync()
    {
        var users =
            await _httpClient
                .GetFromJsonAsync<
                    List<UserViewModel>>(
                    "api/users");

        return users ??
            new List<UserViewModel>();
    }

    // ============================================================
    // Get comments
    // ============================================================

    public async Task<List<CommentViewModel>>
        GetCommentsAsync(int ticketId)
    {
        var comments =
            await _httpClient
                .GetFromJsonAsync<
                    List<CommentViewModel>>(
                    $"api/tickets/{ticketId}/comments");

        return comments ??
            new List<CommentViewModel>();
    }

    // ============================================================
    // Create ticket
    // ============================================================

    public async Task<TicketViewModel?>
        CreateTicketAsync(
            CreateTicketApiRequest request)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "api/tickets",
                request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<TicketViewModel>();
    }

    // ============================================================
    // Add comment
    // ============================================================

    public async Task<CommentViewModel?>
        AddCommentAsync(
            int ticketId,
            CreateCommentViewModel request)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                $"api/tickets/{ticketId}/comments",
                request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content
            .ReadFromJsonAsync<CommentViewModel>();
    }

    // ============================================================
    // Update status
    // ============================================================

    public async Task<bool>
        UpdateTicketStatusAsync(
            int ticketId,
            int statusId)
    {
        var request =
            new UpdateStatusViewModel
            {
                StatusId = statusId
            };

        var response =
            await _httpClient.PutAsJsonAsync(
                $"api/tickets/{ticketId}/status",
                request);

        return response.IsSuccessStatusCode;
    }

    // ============================================================
    // Assign ticket
    // ============================================================

    public async Task<bool>
        AssignTicketAsync(
            int ticketId,
            int assignedToId)
    {
        var request =
            new AssignTicketViewModel
            {
                AssignedToId = assignedToId
            };

        var response =
            await _httpClient.PutAsJsonAsync(
                $"api/tickets/{ticketId}/assign",
                request);

        return response.IsSuccessStatusCode;
    }
}
