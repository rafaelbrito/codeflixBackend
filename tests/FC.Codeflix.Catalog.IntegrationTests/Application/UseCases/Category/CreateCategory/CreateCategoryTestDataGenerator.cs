namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory
{
    public class CreateCategoryTestDataGenerator
    {
        public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
        {
            var fixture = new CreateCategoryTestFixture();
            var invalidInputsList = new List<object[]>();
            int totalInvalidCases = 4;

            for (int i = 0; i < times; i++)
            {
                switch (i % totalInvalidCases)
                {
                    case 0:
                        invalidInputsList.Add(new object[] {fixture.GetInvalidInputShortName(),
                "Name should be at least 3 characters long"
                        });
                        break;
                    case 1:
                        invalidInputsList.Add(new object[] { fixture.GetInvalitInputTooLongName(),
                "Name should be least or equal 255 characters long"
                        });
                        break;
                    case 2:
                        invalidInputsList.Add(new object[] { fixture.GetInvalidInputCategoryNull(),
                "Description should not be null"
                        });
                        break;
                    case 3:
                        invalidInputsList.Add(new object[] { fixture.GetInvalidInputTooLongDescription(),
                "Description should be least or equal 10000 characters long"
                        });
                        break;
                }
            }
            return invalidInputsList;
        }
    }
}
