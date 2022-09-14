﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.LanguageServer.Common;
using Microsoft.AspNetCore.Razor.LanguageServer.EndpointContracts;
using Microsoft.AspNetCore.Razor.LanguageServer.Protocol;
using Microsoft.CodeAnalysis.Razor.Workspaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace Microsoft.AspNetCore.Razor.LanguageServer.Hover
{
    internal class RazorHoverEndpoint : AbstractRazorDelegatingEndpoint<TextDocumentPositionParams, VSInternalHover?, DelegatedPositionParams>, IVSHoverEndpoint
    {
        private readonly RazorHoverInfoService _hoverInfoService;
        private readonly RazorDocumentMappingService _documentMappingService;
        private VSInternalClientCapabilities? _clientCapabilities;

        public RazorHoverEndpoint(
            RazorHoverInfoService hoverInfoService,
            LanguageServerFeatureOptions languageServerFeatureOptions,
            RazorDocumentMappingService documentMappingService,
            ClientNotifierServiceBase languageServer,
            ILoggerFactory loggerFactory)
            : base(languageServerFeatureOptions, documentMappingService, languageServer, loggerFactory.CreateLogger<RazorHoverEndpoint>())
        {
            _hoverInfoService = hoverInfoService ?? throw new ArgumentNullException(nameof(hoverInfoService));
            _documentMappingService = documentMappingService ?? throw new ArgumentNullException(nameof(documentMappingService));
        }

        public RegistrationExtensionResult GetRegistration(VSInternalClientCapabilities clientCapabilities)
        {
            const string AssociatedServerCapability = "hoverProvider";
            _clientCapabilities = clientCapabilities;

            var registrationOptions = new HoverOptions()
            {
                WorkDoneProgress = false,
            };

            return new RegistrationExtensionResult(AssociatedServerCapability, new SumType<bool, HoverOptions>(registrationOptions));
        }

        /// <inheritdoc/>
        protected override string CustomMessageTarget => RazorLanguageServerCustomMessageTargets.RazorHoverEndpointName;

        public override bool MutatesSolutionState => false;

        /// <inheritdoc/>
        protected override IDelegatedParams CreateDelegatedParams(TextDocumentPositionParams request, RazorRequestContext razorRequestContext, Projection projection, CancellationToken cancellationToken)
        {
            var documentContext = razorRequestContext.GetRequiredDocumentContext();
            return new DelegatedPositionParams(
                    documentContext.Identifier,
                    projection.Position,
                    projection.LanguageKind);
        }

        /// <inheritdoc/>
        protected override async Task<VSInternalHover?> TryHandleAsync(TextDocumentPositionParams request, RazorRequestContext razorRequestContext, Projection projection, CancellationToken cancellationToken)
        {
            var documentContext = razorRequestContext.GetRequiredDocumentContext();
            // HTML can still sometimes be handled by razor. For example hovering over
            // a component tag like <Counter /> will still be in an html context
            if (projection.LanguageKind == RazorLanguageKind.CSharp)
            {
                return null;
            }

            var location = new SourceLocation(projection.AbsoluteIndex, request.Position.Line, request.Position.Character);
            var codeDocument = await documentContext.GetCodeDocumentAsync(cancellationToken);

            return _hoverInfoService.GetHoverInfo(codeDocument, location, _clientCapabilities!);
        }

        /// <inheritdoc/>
        protected override async Task<VSInternalHover?> HandleDelegatedResponseAsync(VSInternalHover? response, RazorRequestContext razorRequestContext, CancellationToken cancellationToken)
        {
            if (response?.Range is null)
            {
                return response;
            }

            var documentContext = razorRequestContext.GetRequiredDocumentContext();
            var codeDocument = await documentContext.GetCodeDocumentAsync(cancellationToken).ConfigureAwait(false);

            if (_documentMappingService.TryMapFromProjectedDocumentRange(codeDocument, response.Range, out var projectedRange))
            {
                response.Range = projectedRange;
            }

            return response;
        }
    }
}
