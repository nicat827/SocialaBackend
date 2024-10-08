using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.Forbidden;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using SocialaBackend.Persistence.Implementations.Hubs;
using SocialaBackend.Persistence.Implementations.Repositories;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class PostService : IPostService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileService _fileService;
        private readonly IPostRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPostRepository _postRepository;

        private readonly string _currentUserName;

        public PostService(UserManager<AppUser> userManager,
            IFileService fileService,
            IPostRepository repository,
            IMapper mapper,
            ICommentRepository commentRepository,
            IHttpContextAccessor http,
            IHubContext<NotificationHub> notificationHubContext,
            ICloudinaryService cloudinaryService,
            INotificationRepository notificationRepository,
            IPostRepository postRepository)
        {
            _userManager = userManager;
            _fileService = fileService;
            _repository = repository;
            _mapper = mapper;
            _currentUserName = http.HttpContext.User.Identity.Name;
            _commentRepository = commentRepository;
            _notificationHubContext = notificationHubContext;
            _cloudinaryService = cloudinaryService;
            _notificationRepository = notificationRepository;
            _postRepository = postRepository;
        }

        public async Task<CommentGetDto> CommentAsync(CommentPostDto dto)
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUserName);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Post post = await _postRepository.GetByIdAsync(dto.Id,true, includes:new[] {"AppUser", "AppUser.Followers" });
            if (post is null) throw new NotFoundException("Post didnt found!");
            AppUser owner = post.AppUser;

            if (owner.IsPrivate)
            {
                if (owner.UserName != _currentUserName && !owner.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing posts!");
            }

            Comment newComment = new Comment
            {
                Text = dto.Text,
                AuthorId = user.Id,
                
            };
           
            if (post.AppUser.PostCommentNotify && user.UserName != post.AppUser.UserName)
            {
                Notification newNotification = new Notification
                {
                    AppUser = owner,
                    Title = "Post Commented!",
                    Text = $"{user.UserName} commented your post!",
                    SourceUrl = user.ImageUrl,
                    Type = NotificationType.Custom,
                    SrcId = post.Id,
                    UserName = user.UserName,
                };
                await _notificationRepository.CreateAsync(newNotification);
                await _notificationRepository.SaveChangesAsync();
                NotificationsGetDto notificationDto = new() {Id= newNotification.Id, IsChecked = false, SrcId = newNotification.SrcId, UserName = newNotification.UserName, SourceUrl = newNotification.SourceUrl, Title = newNotification.Title, Text = newNotification.Text, CreatedAt = DateTime.Now, Type = newNotification.Type.ToString() };
                await _notificationHubContext.Clients.Group(post.AppUser.UserName).SendAsync("NewNotification", notificationDto);
            }
            post.Comments.Add(newComment);
            post.CommentsCount++;
            await _postRepository.SaveChangesAsync();
            return new CommentGetDto
            (
                newComment.Id,
                newComment.Author.UserName,
                newComment.Author.ImageUrl,
                newComment.Text,
                newComment.Likes.Count,
                0,
                newComment.CreatedAt,
                newComment.ParentCommentId
            );

        }
        public async Task<IEnumerable<CommentGetDto>> GetRepliesAsync(int id, int? skip)
        {
            if (skip is null) skip = 0;
            Comment? comment = await _commentRepository.GetByIdAsync(id, includes: new[] { "Post", "Post.AppUser", "Post.AppUser.Followers" });
            if (comment is null) throw new NotFoundException($"Comment with id {id} wasnt defined!");
            AppUser owner = comment.Post.AppUser;
            if (owner.IsPrivate)
            {
                if (owner.UserName != _currentUserName && !owner.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            IEnumerable<Comment> replies = await _commentRepository.GetCollection(c => c.ParentCommentId == comment.Id, (int)skip, 10, includes: "Author");
            int repliesCount = 0;
            ICollection<CommentGetDto> commentsDto = new List<CommentGetDto>();
            foreach (Comment reply in replies)
            {
                repliesCount = await _commentRepository.GetCountAsync(c => c.ParentCommentId == reply.Id);
                commentsDto.Add(new CommentGetDto
                (
                    reply.Id,
                    reply.Author.UserName,
                    reply.Author.ImageUrl,
                    reply.Text,
                    reply.Likes.Count,
                    repliesCount,
                    reply.CreatedAt,
                    null

                ));
            }
            return commentsDto;
        }
        public async Task<CommentGetDto> AddReplyAsync(ReplyPostDto dto)
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUserName);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Comment comment = await _commentRepository.GetByIdAsync(dto.CommentId,true, includes:new[] { "Post", "Post.AppUser" });
            if (comment is null) throw new NotFoundException("Comment didnt found!");
            Post post = comment.Post;
            AppUser owner = post.AppUser;

            if (owner.IsPrivate)
            {
                if (owner.UserName != _currentUserName && !owner.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing posts!");
            }

            Comment newComment = new Comment
            {
                Text = dto.Text,
                AuthorId = user.Id,
                ParentCommentId = dto.CommentId

            };

            post.Comments.Add(newComment);
            post.CommentsCount++;
            await _postRepository.SaveChangesAsync();
            return new CommentGetDto
            (
                newComment.Id,
                newComment.Author.UserName,
                newComment.Author.ImageUrl,
                newComment.Text,
                newComment.Likes.Count,
                0,
                newComment.CreatedAt,
                newComment.ParentCommentId
            );


        }

        public async Task DeleteCommentAsync(int id)
        {
            Comment? comment = await _commentRepository.GetByIdAsync(id, includes:new[] { "Author", "Post" });
            if (comment is null) throw new NotFoundException($"Comment with id {id} didnt found!");
            if (comment.Author?.UserName != _currentUserName) throw new DontHavePermissionException($"You cant delete this comment!");
            ICollection<Comment> replies = await _commentRepository.GetCollection(c => c.ParentCommentId == id);
            foreach (Comment reply in replies)
            {
                _commentRepository.Delete(reply);
                await _commentRepository.SaveChangesAsync();

            }
            _commentRepository.Delete(comment);
            await _commentRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<PostGetDto>> GetArchivedPostsAsync(int skip)
        {

            IEnumerable<Post> posts = await _postRepository.GetCollection(
                p => p.IsDeleted && p.AppUser.UserName == _currentUserName,
                skip: skip,
                take: 10,
                iqnoreQuery: true,
                expressionIncludes: p => p.Comments.Take(5),
                includes: new[] { "AppUser", "Comments.Author", "Items" });
            ICollection<PostGetDto> dto = new List<PostGetDto>();
            foreach (Post post in posts)
            {
                int commentsCount = await _commentRepository.GetCountAsync(c => c.PostId == post.Id);
                int repliesCount = 0;
                foreach (Comment comment in post.Comments)
                {
                    repliesCount = await _commentRepository.GetCountAsync(c => c.ParentCommentId == comment.Id);
                }
               
                dto.Add(new PostGetDto
                (
                    post.Id,
                    post.AppUser.UserName,
                    post.AppUser.Name,
                    post.AppUser.Surname,
                    post.AppUser.ImageUrl,
                    post.Description,
                    post.CreatedAt,
                    _mapper.Map<IEnumerable<PostItemGetDto>>(post.Items),
                    _mapper.Map<IEnumerable<CommentGetDto>>(post.Comments),
                    commentsCount,
                    post.LikesCount
                ));
            }
            return dto;


        }
        public async Task LikeCommentAsync(int id)
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUserName);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Comment comment = await _commentRepository.GetByIdAsync(id,true, includes: new[] { "Post", "Post.AppUser", "Post.AppUser.Followers" });
            if (comment is null) throw new NotFoundException("Comment didnt found!");
            AppUser owner = comment.Post.AppUser;

            if (owner.IsPrivate)
            {
                if (owner.UserName != _currentUserName && !owner.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            CommentLikeItem? likedItem = comment.Likes.FirstOrDefault(li => li.AppUserId == user.Id);
            if (likedItem is null)
            {
                comment.Likes.Add(new CommentLikeItem { AppUserId = user.Id });
            }
            else
            {
                comment.Likes.Remove(likedItem);
                
            }
            await _repository.SaveChangesAsync();
        }


        public async Task<PostGetDto> CreatePostAsync(PostPostDto dto)
        {
            if (dto.Description is null && dto.Files is null) throw new PostCreateException("At least one of the fields is required!");
            AppUser user = await _getUser(_currentUserName);
            Post newPost = new Post
            {
                Description = dto.Description,
                Items = new List<PostItem>(),
                AppUserId = user.Id
            };
            if (dto.Files is not null) {
                foreach (var file in dto.Files)
                {
                    _fileService.CheckFileSize(file, 100);
                    FileType type = _fileService.ValidateFilesForPost(file);
                    string localSourceUrl = await _fileService.CreateFileAsync(file, "uploads", "posts", $"{type}s");
                    string cloudinarySrcUrl = await _cloudinaryService.UploadFileAsync(localSourceUrl, type, "uploads", "posts", $"{type}s");
                    newPost.Items.Add(new PostItem { Type = type, SourceUrl = cloudinarySrcUrl });
                }
            }
            

            await _repository.CreateAsync(newPost);
            await _repository.SaveChangesAsync();
            PostGetDto returnDto = new PostGetDto(
                newPost.Id,
                newPost.AppUser.UserName,
                newPost.AppUser.Name,
                newPost.AppUser.Surname,
                newPost.AppUser.ImageUrl,
                newPost.Description,
                newPost.CreatedAt,
                _mapper.Map<IEnumerable<PostItemGetDto>>(newPost.Items),
                new List<CommentGetDto>(),
                0,
                newPost.LikesCount
            );
          
            return returnDto;

        }

        public async Task<IEnumerable<CommentGetDto>> GetCommentsAsync(int id, int? skip)
        {
            if (skip is null) skip = 0;
            Post? post = await _postRepository.GetByIdAsync(id, includes: new[] { "AppUser", "AppUser.Followers" });
            if (post is null) throw new NotFoundException($"Post with id {id} wasnt defined!");
            if (post.AppUser.IsPrivate)
            {
                if (post.AppUser.UserName != _currentUserName)
                    if (!post.AppUser.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                        throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            IEnumerable<Comment> comments = await _commentRepository.GetCollection(c => c.PostId == post.Id && c.ParentCommentId == null, (int)skip, 10, includes:new[] { "Author", "Likes" });
            ICollection<CommentGetDto> dto = new List<CommentGetDto>();
            foreach (Comment comment in comments)
            {
                int repliesCount = await _commentRepository.GetCountAsync(c => c.ParentCommentId == comment.Id);
                dto.Add(new CommentGetDto(comment.Id, comment.Author.UserName, comment.Author.ImageUrl,comment.Text, comment.Likes.Count, repliesCount, comment.CreatedAt, null));
            }
            return dto;
        }

        public async Task<ICollection<PostLikeGetDto>> GetLikesAsync(int id, int? skip)
        
        {
            if (skip is null) skip = 0;
            Post? post = await _postRepository.GetByIdAsync(id, expressionIncludes: p => p.Likes.Skip((int)skip), includes: new[] { "AppUser", "AppUser.Followers", "Likes","Likes.LikedUser" });
          
            if (post is null) throw new NotFoundException($"Post with id {id} wasnt defined!");
            if (post.AppUser.IsPrivate)
            {
                if (post.AppUser.UserName != _currentUserName)
                    if (!post.AppUser.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                        throw new ForbiddenException("This account is private, follow for seeing post likes!");
            }
            ICollection<PostLikeGetDto> dto = new List<PostLikeGetDto>();
            foreach (PostLikeItem likeItem in post.Likes)
            {
                dto.Add(new PostLikeGetDto(
                    likeItem.Id,
                    likeItem.LikedUser.UserName,
                    likeItem.LikedUser.Name,
                    likeItem.LikedUser.Surname,
                    likeItem.LikedUser.ImageUrl
                ));
            }
            return dto;

        }

        public async Task<ICollection<PostGetDto>> GetPostsAsync(string username)
        {
            AppUser? user = await _userManager.Users
                .Where(u => u.UserName == username)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Comments.Where(c => c.ParentCommentId == null).Take(5))
                        .ThenInclude(c => c.Author)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Items)
                .Include(u => u.Followers)
                .FirstOrDefaultAsync();
          
            if (user is null) throw new AppUserNotFoundException($"User with {username} username doesnt exists!");
            if (user.IsPrivate)
            {
                if (username != _currentUserName)
                    if (!user.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                        throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            ICollection<PostGetDto> dto = new List<PostGetDto>();
            foreach (Post post in user.Posts)
            {
                int repliesCount = 0;
                ICollection<CommentGetDto> commentsDto = new List<CommentGetDto>();
                int commentsCount = await _commentRepository.GetCountAsync(c => c.PostId == post.Id);

                foreach (Comment comment in post.Comments)
                {

                    repliesCount = await _commentRepository.GetCountAsync(c => c.ParentCommentId == comment.Id);
                    commentsDto.Add(new CommentGetDto
                    (
                        comment.Id,
                        comment.Author.UserName,
                        comment.Author.ImageUrl,
                        comment.Text,
                        comment.Likes.Count,
                        repliesCount,
                        comment.CreatedAt,
                        null

                    ));
                }
               
                dto.Add(new PostGetDto
                (
                    post.Id,
                    post.AppUser.UserName,
                    post.AppUser.Name,
                    post.AppUser.Surname,
                    post.AppUser.ImageUrl,
                    post.Description,
                    post.CreatedAt,
                    _mapper.Map<IEnumerable<PostItemGetDto>>(post.Items),
                    commentsDto,
                    commentsCount,
                    post.LikesCount
                ));
            }
            return dto;

        }

        public async Task<IEnumerable<PostGetDto>> GetFeedPostsAsync(int skip)
        {
            ICollection<Post> posts = await _postRepository.OrderAndGet
                (
                    order: p => p.CreatedAt,
                    isDescending: true,
                    expression: p => p.AppUser.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed),
                    skip: skip,
                    limit: 10,
                    expressionIncludes: p=> p.Comments.Where(c => c.ParentCommentId == null).Take(5),
                    includes: new[] { "Items","AppUser", "AppUser.Followers", "Comments.Author", "Comments.Likes" }
                ).ToListAsync();

            ICollection<PostGetDto> dto = new List<PostGetDto>();
            foreach (Post post in posts)
            {
                int repliesCount = 0;
                int commentsCount = await _commentRepository.GetCountAsync(c => c.PostId == post.Id);

                ICollection<CommentGetDto> commentsDto = new List<CommentGetDto>();
                foreach (Comment comment in post.Comments)
                {
                    repliesCount = await _commentRepository.GetCountAsync(c => c.ParentCommentId == comment.Id);
                    commentsDto.Add(new CommentGetDto
                    (
                        comment.Id,
                        comment.Author.UserName,
                        comment.Author.ImageUrl,
                        comment.Text,
                        comment.Likes.Count,
                        repliesCount,
                        comment.CreatedAt,
                        null

                    ));
                }

                dto.Add(new PostGetDto
                (
                    post.Id,
                    post.AppUser.UserName,
                    post.AppUser.Name,
                    post.AppUser.Surname,
                    post.AppUser.ImageUrl,
                    post.Description,
                    post.CreatedAt,
                    _mapper.Map<IEnumerable<PostItemGetDto>>(post.Items),
                    commentsDto,
                    commentsCount,
                    post.LikesCount
                ));
            }
            return dto;

        }

        public async Task LikePostAsync(int id)
        {
            Post post = await _postRepository.GetByIdAsync(id, true, includes: new[] {"Likes", "Likes.LikedUser", "AppUser", "AppUser.Followers" });
           
            if (post is null) throw new NotFoundException("Post didnt found!");
            if (post.AppUser.IsPrivate)
            {
                if (post.AppUser.UserName != _currentUserName)
                    if (!post.AppUser.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                        throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            PostLikeItem? likedItem = post.Likes.FirstOrDefault(li => li.LikedUser.UserName == _currentUserName);
            if (likedItem is null)
            {
                AppUser user = await _getUser(_currentUserName);
                post.Likes.Add(new PostLikeItem { LikedUserId = user.Id});
                post.LikesCount++;
                if (post.AppUser.PostLikeNotify && user.UserName != post.AppUser.UserName)
                {
                    Notification newNotification = new Notification
                    {
                        AppUser = post.AppUser,
                        Title = "Post Liked!",
                        Text = $"{user.UserName} liked your post!",
                        SourceUrl = user.ImageUrl,
                        Type = NotificationType.Custom,
                        SrcId = post.Id,
                        UserName = user.UserName,
                        
                    };
                    await _notificationRepository.CreateAsync(newNotification);
                    await _notificationRepository.SaveChangesAsync();
                    NotificationsGetDto dto = new() {Id=newNotification.Id, IsChecked = false, SrcId = newNotification.SrcId, UserName = newNotification.UserName, SourceUrl = newNotification.SourceUrl, Title = newNotification.Title, Text = newNotification.Text, CreatedAt = DateTime.Now, Type = newNotification.Type.ToString()};
                    await _notificationHubContext.Clients.Group(post.AppUser.UserName).SendAsync("NewNotification",  dto);
                }
            }
            else
            {
                post.Likes.Remove(likedItem);
                post.LikesCount--;

            }
            await _repository.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int id)
        {
            AppUser? currentUser = await _userManager.Users.IgnoreQueryFilters().Where(u => u.UserName == _currentUserName).Include(u => u.Posts.Where(p => p.Id == id)).FirstOrDefaultAsync();
            if (currentUser is null) throw new AppUserNotFoundException("User is not defined!");
            if (currentUser.Posts.FirstOrDefault() is null) throw new NotFoundException($"You dont have a post with id: {id}!");
            Post? post = await _postRepository.GetByIdAsync(id, true,true, includes:new[] { "Likes",
                                                                        "Comments", "Comments.Likes", "Comments.Replies", "Comments.Replies.Likes"});
            
            if (post.IsDeleted)
            {
                _postRepository.Delete(post);
            }
            else
            {
                post.IsDeleted = true;
                foreach (var like in  post.Likes)
                {
                    like.IsDeleted = true;
                }
                foreach (Comment comment in post.Comments)
                {
                    comment.IsDeleted = true;
                    foreach (var commentLike in comment.Likes)
                    {
                        commentLike.IsDeleted = true;
                    }
                }
            }
            await _postRepository.SaveChangesAsync();
        }

        public async Task RecoverPostAsync(int id)
        {
            AppUser? currentUser = await _userManager.Users.IgnoreQueryFilters().Where(u => u.UserName == _currentUserName).Include(u => u.Posts.Where(p => p.Id == id)).FirstOrDefaultAsync();
            if (currentUser is null) throw new AppUserNotFoundException("User is not defined!");
            if (currentUser.Posts.FirstOrDefault() is null) throw new NotFoundException($"You dont have a post with id: {id}!");
            Post? post = await _postRepository.GetByIdAsync(id, true, true, includes: new[] { "Likes",
                                                                        "Comments", "Comments.Likes", "Comments.Replies", "Comments.Replies.Likes"});
            if (post.IsDeleted)
            {
                post.IsDeleted = false;
                foreach (var like in post.Likes)
                {
                    like.IsDeleted = false;
                }
                foreach (Comment comment in post.Comments)
                {
                    comment.IsDeleted =false;
                    foreach (var commentLike in comment.Likes)
                    {
                        commentLike.IsDeleted = false;
                    }
                    
                }
            }

            await _postRepository.SaveChangesAsync();
           
        }


        private async Task<AppUser> _getUser(string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException($"User with {username} username doesnt exists!");
            return user;
        }

    }
}
