using StoryTeller.Execution;

namespace StoryTellerTestHarness
{
    public class Template
    {
        public void Can_verify_the_displayed_columns_by_name()
        {
            var runner = new ProjectTestRunner(@"C:\code\FubuMVC.SlickGrid\src\SlickGridStoryteller\storyteller.xml");

            try
            {
                
                runner.RunAndAssertTest("Simple/Can verify the displayed columns by name");
            }
            finally
            {
                runner.Dispose();
            }

            
        }
    }
}