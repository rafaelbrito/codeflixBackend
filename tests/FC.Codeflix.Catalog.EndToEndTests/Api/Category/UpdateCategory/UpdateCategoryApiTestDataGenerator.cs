namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

public class UpdateCategoryApiTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs()
    {
        var fixture = new UpdateCategoryApiTestFixture();
        var invalidInputsList = new List<object[]>();
        int totalInvalidCases = 3;

        for (int i = 0; i < totalInvalidCases; i++)
        {
            switch (i % totalInvalidCases)
            {
                case 0:
                    var input0 = fixture.GetExampleInput();
                    input0.Name = fixture.GetInvalidTooShortName();
                    invalidInputsList.Add(new object[] {input0,
                    "Name should be at least 3 characters long"
                    });
                    break;
                case 1:
                    var input1 = fixture.GetExampleInput();
                    input1.Name = fixture.GetInvalitInputTooLongName();
                    invalidInputsList.Add(new object[] { input1,
                    "Name should be least or equal 255 characters long"
                    });
                    break;
                case 2:
                    var input2 = fixture.GetExampleInput();
                    input2.Description = fixture.GetInvalidDescriptionTooLong();
                    invalidInputsList.Add(new object[] { input2,
                    "Description should be least or equal 10000 characters long"
                    });
                    break;
                default:
                    break;
            }
        }
        return invalidInputsList;
    }
}
