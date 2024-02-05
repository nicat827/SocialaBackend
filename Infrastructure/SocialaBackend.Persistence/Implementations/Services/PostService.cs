using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Application.Exceptions;
using SocialaBackend.Application.Exceptions.Forbidden;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Services
{
    internal class PostService : IPostService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFIleService _fileService;
        private readonly IPostRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly IReplyRepository _replyRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;

        private readonly string _currentUserName;

        public PostService(UserManager<AppUser> userManager,
            IFIleService fileService,
            IPostRepository repository,
            IMapper mapper,
            ICommentRepository commentRepository,
            IReplyRepository replyRepository,
            IHttpContextAccessor http,
            ILikeRepository likeRepository,
            IPostRepository postRepository)
        {
            _userManager = userManager;
            _fileService = fileService;
            _repository = repository;
            _mapper = mapper;
            _currentUserName = http.HttpContext.User.Identity.Name;
            _commentRepository = commentRepository;
            _replyRepository = replyRepository;
            _likeRepository = likeRepository;
            _postRepository = postRepository;
        }

        public async Task CommentAsync(int id, string text)
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUserName);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Post post = await _postRepository.GetByIdAsync(id,true, includes:new[] { "Comments", "AppUser", "AppUser.Followers" });
            if (post is null) throw new NotFoundException("Post didnt found!");
            AppUser owner = post.AppUser;

            if (owner.IsPrivate)
            {
                if (owner.UserName != _currentUserName && !owner.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            post.Comments.Add(new Comment
            {
                Text = text,
                AuthorImageUrl = user.ImageUrl,
                Author = user.UserName
            });
            post.CommentsCount++;
            await _postRepository.SaveChangesAsync();

        }
        public async Task LikeReplyAsync(int id)
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUserName);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Reply reply = await _replyRepository.GetByIdAsync(id,true, includes: new[] { "Likes", "Comment", "Comment.Post", "Comment.Post.AppUser", "Comment.Post.AppUser.Followers" });
            if (reply is null) throw new NotFoundException("Reply didnt found!");

            AppUser owner = reply.Comment.Post.AppUser;

            if (owner.IsPrivate)
            {
                if (owner.UserName != _currentUserName && !owner.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            ReplyLikeItem? likedItem = reply.Likes.FirstOrDefault(li => li.AppUserId == user.Id);
            if (likedItem is null)
            {
                reply.Likes.Add(new ReplyLikeItem { AppUserId = user.Id });
                reply.LikesCount++;
            }
            else
            {
                reply.Likes.Remove(likedItem);
                reply.LikesCount--;

            }
            await _replyRepository.SaveChangesAsync();
        }
        public async Task LikeCommentAsync(int id)
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUserName);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Comment comment = await _commentRepository.GetByIdAsync(id,true, includes: new[] { "Likes", "Post", "Post.AppUser", "Post.AppUser.Followers" });
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
                comment.LikesCount++;
            }
            else
            {
                comment.Likes.Remove(likedItem);
                comment.LikesCount--;

            }
            await _repository.SaveChangesAsync();
        }
        public async Task ReplyCommentAsync(int id, string text)
        {
            AppUser user = await _userManager.FindByNameAsync(_currentUserName);
            if (user is null) throw new AppUserNotFoundException("User wasnt found!");
            Comment? comment = await _commentRepository.GetByIdAsync(id,true, includes: new[] { "Replies", "Post", "Post.AppUser", "Post.AppUser.Followers" });
            if (comment is null) throw new NotFoundException("Comment didnt found!");
            AppUser owner = comment.Post.AppUser;
            if (owner.IsPrivate)
            {
                if (owner.UserName != _currentUserName && !owner.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            comment.Replies.Add(new Reply
            {
                Text = text,
                AuthorImageUrl = user.ImageUrl,
                Author = user.UserName
            });
          
            comment.RepliesCount++;
            await _commentRepository.SaveChangesAsync();

        }
        public async Task<IEnumerable<ReplyGetDto>> GetRepliesAsync(int id, int? skip)
        {
            if (skip is null) skip = 0;
            Comment? comment = await _commentRepository.GetByIdAsync(id,expression:c => c.Replies.Skip((int)skip).Take(10), includes: new[] { "Post", "Post.AppUser", "Post.AppUser.Followers" });
            if (comment is null) throw new NotFoundException($"Comment with id {id} wasnt defined!");
            AppUser owner = comment.Post.AppUser;
            if (owner.IsPrivate)
            {
                if (owner.UserName != _currentUserName && !owner.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                    throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            return _mapper.Map<IEnumerable<ReplyGetDto>>(comment.Replies);

        }


        public async Task CreatePostAsync(PostPostDto dto)
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
                    _fileService.CheckFileSize(file, 15);
                    _fileService.ValidateFilesForPost(file);
                    string type = file.ContentType.Substring(0, file.ContentType.IndexOf("/"));
                    newPost.Items.Add(new PostItem { SourceUrl = await _fileService.CreateFileAsync(file, "uploads", "posts", $"{type}s") });
                }
            }
        

            await _repository.CreateAsync(newPost);
            await _repository.SaveChangesAsync();

        }

        public async Task<IEnumerable<CommentGetDto>> GetCommentsAsync(int id, int? skip)
        {
            if (skip is null) skip = 0;
            Post? post = await _postRepository.GetByIdAsync(id, expression:p => p.Comments.Skip((int)skip).Take(10), includes: new[] { "AppUser", "AppUser.Followers" });
            if (post is null) throw new NotFoundException($"Post with id {id} wasnt defined!");
            if (post.AppUser.IsPrivate)
            {
                if (post.AppUser.UserName != _currentUserName)
                    if (!post.AppUser.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                        throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            return _mapper.Map<IEnumerable<CommentGetDto>>(post.Comments);

        }

        public async Task<IEnumerable<PostLikeGetDto>> GetLikesAsync(int id, int? skip)
        
        {
            if (skip is null) skip = 0;
            Post? post = await _postRepository.GetByIdAsync(id, expression: p => p.Likes.Skip((int)skip),includes: new[] { "AppUser", "AppUser.Followers" });
            if (post is null) throw new NotFoundException($"Post with id {id} wasnt defined!");
            if (post.AppUser.IsPrivate)
            {
                if (post.AppUser.UserName != _currentUserName)
                    if (!post.AppUser.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                        throw new ForbiddenException("This account is private, follow for seeing post likes!");
            }
            return _mapper.Map<IEnumerable<PostLikeGetDto>>(post.Likes);

        }

        public async Task<ICollection<PostGetDto>> GetPostsAsync(string username)
        {
            AppUser? user = await _userManager.Users
                .Where(u => u.UserName == username)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Comments.Take(5))
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
            ICollection<PostGetDto> dto = _mapper.Map<ICollection<PostGetDto>>(user.Posts);
            
            return dto;

        }

        public async Task LikePostAsync(int id)
        {
           
            Post post = await _postRepository.GetByIdAsync(id, true, includes: new[] { "Likes", "AppUser", "AppUser.Followers" });
            if (post is null) throw new NotFoundException("Post didnt found!");
            if (post.AppUser.IsPrivate)
            {
                if (post.AppUser.UserName != _currentUserName)
                    if (!post.AppUser.Followers.Any(f => f.UserName == _currentUserName && f.IsConfirmed == true))
                        throw new ForbiddenException("This account is private, follow for seeing posts!");
            }
            PostLikeItem? likedItem = post.Likes.FirstOrDefault(li => li.Username == _currentUserName);
            if (likedItem is null)
            {
                AppUser user = await _getUser(_currentUserName);
                post.Likes.Add(new PostLikeItem {AppUserId=user.Id, Username = user.UserName, ImageUrl = user.ImageUrl, Name = user.Name, Surname = user.Surname });
                post.LikesCount++;
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
                    foreach (Reply reply in comment.Replies)
                    {
                        reply.IsDeleted = true;
                        foreach (var replyLike in reply.Likes)
                        {
                            replyLike.IsDeleted = true;
                        }
                    }
                }
            }
            await _postRepository.SaveChangesAsync();
        }

        public Task RecoverPostAsync(int id)
        {
            throw new NotImplementedException();
        }
        private async Task<AppUser> _getUser(string username)
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user is null) throw new AppUserNotFoundException($"User with {username} username doesnt exists!");
            return user;
        }

    }
}
