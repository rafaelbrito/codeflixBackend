﻿using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GenresController(IMediator mediator)
         => _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create([FromBody] CreateGenreInput input, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(input, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = output.Id },
                new ApiResponse<GenreModelOutput>(output));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new GetGenreInput(id), cancellationToken);
            return Ok(new ApiResponse<GenreModelOutput>(output));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteGenreInput(id), cancellationToken);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update([FromBody] UpdateGenreApiInput input,
            [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new UpdateGenreInput(
                id, 
                input.Name, 
                input.IsActive, 
                input.CategoriesIds
                ), 
                cancellationToken);
            return Ok(new ApiResponse<GenreModelOutput>(output));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListGenresOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> List(
            CancellationToken cancellationToken,
            [FromQuery] int? page = null,
            [FromQuery(Name = "per_page")] int? perPage = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sort = null,
            [FromQuery] SearchOrder? dir = null
                )
        {
            var input = new ListGenresInput();
            if (page is not null) input.Page = page.Value;
            if (perPage is not null) input.PerPage = perPage.Value;
            if (!String.IsNullOrWhiteSpace(search)) input.Search = search;
            if (!String.IsNullOrWhiteSpace(sort)) input.Sort = sort;
            if (dir is not null) input.Dir = dir.Value;

            var output = await _mediator.Send(input, cancellationToken);
            return Ok(new ApiResponseList<GenreModelOutput>(output));
        }
    }
}
