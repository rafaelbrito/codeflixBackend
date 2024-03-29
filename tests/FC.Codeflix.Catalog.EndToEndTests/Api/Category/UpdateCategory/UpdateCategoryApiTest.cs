﻿using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryApiTestFixture))]
    public class UpdateCategoryApiTest : IDisposable
    {
        private readonly UpdateCategoryApiTestFixture _fixture;

        public UpdateCategoryApiTest()
         => _fixture = new UpdateCategoryApiTestFixture();

        [Fact(DisplayName = (nameof(UpdateCategory)))]
        [Trait("EndToEnd/API", "Category/Update - EndPoints")]
        public async Task UpdateCategory()
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var exampleCategory = exampleCategoryList[10];
            var input = _fixture.GetExampleInput();

            var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(input.Description);
            output.Data.IsActive.Should().Be((bool)input.IsActive!);

            var dbCategory = await _fixture.Persistence.GetById(exampleCategory.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be((bool)input.IsActive);
        }

        [Fact(DisplayName = (nameof(UpdateCategoryOnlyName)))]
        [Trait("EndToEnd/API", "Category/Update - EndPoints")]
        public async Task UpdateCategoryOnlyName()
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var exampleCategory = exampleCategoryList[10];
            var input = new UpdateCategoryApiInput(_fixture.GetValidCategoryName());

            var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(exampleCategory.Description);
            output.Data.IsActive.Should().Be((bool)exampleCategory.IsActive);

            var dbCategory = await _fixture.Persistence.GetById(exampleCategory.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(exampleCategory.Description);
            dbCategory.IsActive.Should().Be((bool)exampleCategory.IsActive);
        }

        [Fact(DisplayName = (nameof(UpdateCategoryNameAndDescription)))]
        [Trait("EndToEnd/API", "Category/Update - EndPoints")]
        public async Task UpdateCategoryNameAndDescription()
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var exampleCategory = exampleCategoryList[10];
            var input = new UpdateCategoryApiInput(
                _fixture.GetValidCategoryName(),
                _fixture.GetValidCategoryDescription()
                );

            var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Data.Id.Should().Be(exampleCategory.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(input.Description);
            output.Data.IsActive.Should().Be((bool)exampleCategory.IsActive);

            var dbCategory = await _fixture.Persistence.GetById(exampleCategory.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be((bool)exampleCategory.IsActive);
        }

        [Fact(DisplayName = (nameof(ErrorWhenNotFound)))]
        [Trait("EndToEnd/API", "Category/Update - EndPoints")]
        public async Task ErrorWhenNotFound()
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var randomGuid = Guid.NewGuid();
            var input = _fixture.GetExampleInput();

            var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>(
                $"/categories/{randomGuid}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Category '{randomGuid}' not found.");
            output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        }

        [Theory(DisplayName = (nameof(ErrorWhenCantInstatiateAggregate)))]
        [Trait("EndToEnd/API", "Category/Update - EndPoints")]
        [MemberData(
            nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs),
            MemberType = typeof(UpdateCategoryApiTestDataGenerator)
            )]
        public async Task ErrorWhenCantInstatiateAggregate(
            UpdateCategoryApiInput input,
            string expectedDetail
            )
        {
            var exampleCategoryList = _fixture.GetExampleCategoryList(20);
            await _fixture.Persistence.InsertList(exampleCategoryList);
            var exampleCategory = exampleCategoryList[10];

            var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>(
                $"/categories/{exampleCategory.Id}",
                input
                );

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output!.Title.Should().Be("One or more validation errors ocurred");
            output.Type.Should().Be("UnprocessableEntity");
            output.Status.Should().Be((int)StatusCodes.Status422UnprocessableEntity);
            output.Detail.Should().Be(expectedDetail);
        }

        public void Dispose()
            => _fixture.ClearPersistence();
    }
}
