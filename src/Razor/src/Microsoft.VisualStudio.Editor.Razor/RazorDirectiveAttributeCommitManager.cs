﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.Razor
{
    internal class RazorDirectiveAttributeCommitManager : IAsyncCompletionCommitManager
    {
        public IEnumerable<char> PotentialCommitCharacters => new[] { ':', '=' };

        public bool ShouldCommitCompletion(IAsyncCompletionSession session, SnapshotPoint location, char typedChar, CancellationToken token)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (!session.Properties.TryGetCompletionItemKinds(out var completionItemKinds))
            {
                return false;
            }

            if (typedChar == ':' && completionItemKinds.Contains(RazorCompletionItemKind.DirectiveParameter))
            {
                // We're already showing completions for directive parameters, meaning there's already a : in existence. Don't allow the commit.
                return false;
            }

            // Directive attribute completion. This class is only ever called for our specific commit characters, allow the commit.

            return true;
        }

        public CommitResult TryCommit(IAsyncCompletionSession session, ITextBuffer buffer, CompletionItem item, char typedChar, CancellationToken token)
        {
            // Do default behavior for the commit. This enables things like typing `=` and letting the Html completion engine finish off the attribute.
            return CommitResult.Unhandled;
        }
    }
}
