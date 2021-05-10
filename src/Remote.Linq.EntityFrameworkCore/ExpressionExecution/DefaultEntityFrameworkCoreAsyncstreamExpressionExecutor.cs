﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    public class DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor : EntityFrameworkCoreAsyncStreamExpressionExecutor<DynamicObject>
    {
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;

        [SecuritySafeCritical]
        public DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
            : this(dbContext.GetQueryableSetProvider(), context, setTypeInformation)
        {
        }

        public DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
            : this(0, queryableProvider, context ?? new EntityFrameworkCoreExpressionTranslatorContext(), setTypeInformation)
        {
        }

        private DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor(int n, Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext context, Func<Type, bool>? setTypeInformation = null)
            : base(queryableProvider, context)
        {
            _mapper = context.ValueMapper;
            _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
        }

        protected override async IAsyncEnumerable<DynamicObject> ConvertResult(IAsyncEnumerable<object?> queryResult)
        {
            await foreach (var item in queryResult.CheckNotNull(nameof(queryResult)))
            {
                yield return _mapper.MapObject(item, _setTypeInformation)
                    ?? DynamicObject.CreateDefault(Context.SystemExpression?.Type);
            }
        }
    }
}