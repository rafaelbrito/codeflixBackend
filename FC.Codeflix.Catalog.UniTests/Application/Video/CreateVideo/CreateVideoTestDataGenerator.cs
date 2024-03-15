using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using System.Collections;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.CreateVideo
{
    public class CreateVideoTestDataGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var _fixture = new CreateVideoTestFixture();
            var invalidInputsList = new List<object[]>();
            int totalInvalidCases = 4;

            for (int i = 0; i < totalInvalidCases * 2; i++)
            {
                switch (i % totalInvalidCases)
                {
                    case 0:
                        invalidInputsList.Add(new object[] {
                            new CreateVideoInput(
                            "",
                            _fixture.GetValidVideoDescription(),
                            _fixture.GetValidYearLauched(),
                            _fixture.GetRandomBoolean(),
                            _fixture.GetRandomBoolean(),
                            _fixture.GetValidVideoDuration(),
                            _fixture.GetRandomRating()
                        ),
                         "'Title' is required"
                        });
                        break;
                    case 1:
                        invalidInputsList.Add(new object[] {
                            new CreateVideoInput(
                            _fixture.GetValidVideoTitle(),
                            "",
                            _fixture.GetValidYearLauched(),
                            _fixture.GetRandomBoolean(),
                            _fixture.GetRandomBoolean(),
                            _fixture.GetValidVideoDuration(),
                            _fixture.GetRandomRating()
                        ),
                         "'Description' is required"
                        });
                        break;
                    case 2:
                        invalidInputsList.Add(new object[] {
                            new CreateVideoInput(
                            _fixture.GetTooLongTitle(),
                            _fixture.GetValidVideoDescription(),
                            _fixture.GetValidYearLauched(),
                            _fixture.GetRandomBoolean(),
                            _fixture.GetRandomBoolean(),
                            _fixture.GetValidVideoDuration(),
                            _fixture.GetRandomRating()
                        ),
                         "'Title' should be less or equal 255 characters long"
                        });
                        break;
                    case 3:
                        invalidInputsList.Add(new object[] {
                            new CreateVideoInput(
                            _fixture.GetValidVideoTitle(),
                            _fixture.GetTooLongDescription(),
                            _fixture.GetValidYearLauched(),
                            _fixture.GetRandomBoolean(),
                            _fixture.GetRandomBoolean(),
                            _fixture.GetValidVideoDuration(),
                            _fixture.GetRandomRating()
                        ),
                         "'Description' should be less or equal 4000 characters long"
                        });
                        break;
                }
            }
            return invalidInputsList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
