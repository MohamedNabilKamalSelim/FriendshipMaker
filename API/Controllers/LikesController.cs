namespace API.Controllers;

[Authorize]
public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;
    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
        _likesRepository = likesRepository;
        _userRepository = userRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var likedUser = await _userRepository.GetUserByUsernameAsync(username);
        if (likedUser is null) return NotFound();

        // Get the current login user that make the like
        var sourceUserId = User.GetUserId();
        // I get him from likeRepository because this methods is include(likedUsers) so I can add the new like to this list
        var sourceUser = await _likesRepository.GetUserWithLikesAsync(sourceUserId);

        if (sourceUser.UserName == username) return BadRequest("You can not like yourself");

        // Check if the user is already liked
        if (await _likesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id) is not null)
            return BadRequest("You are already liked " + likedUser.UserName);

        // Add the like
        var userLike = new UserLike { SourceUserId = sourceUserId, LikedUserId = likedUser.Id };
        sourceUser.LikedUsers.Add(userLike);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Error while adding the like");

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetLikedUsers([FromQuery] LikesParams likesParams)
    {
        // To set the userId in likesParams object
        likesParams.UserId = User.GetUserId();

        // this will return the members as well as the pagination information
        var users = await _likesRepository.GetUserLikesAsync(likesParams);

        // To send the pagination information in the response header to the client
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

        // then send the members in the body of the response
        return Ok(users);
    }

}