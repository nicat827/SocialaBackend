﻿using SocialaBackend.Application.Abstractions.Repositories;
using SocialaBackend.Domain.Entities;
using SocialaBackend.Persistence.DAL;
using SocialaBackend.Persistence.Implementations.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Persistence.Implementations.Repositories
{
    internal class ReplyRepository : GenericRepository<Reply>, IReplyRepository
    {
        public ReplyRepository(AppDbContext context) : base(context)
        {
        }
    }
}
