using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using weka.classifiers;
using weka.classifiers.evaluation;
using weka.core;

namespace GithubSuccessPredictor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageComboBox.SelectedIndex <0 ||
                ContributersTextBox?.Text =="" ||
                CommitsTextBox?.Text == "" ||
                StarsTextBox?.Text == "" ||
                ForksTextBox?.Text == "" ||
                BranchesTextBox?.Text == "" ||
                WatchersTextBox?.Text == "" ||
                PullRequestsTextBox?.Text == "" ||
                TotalIssuesTextBox?.Text == "" ||
                OpenIssuesTextBox?.Text == "" ||
                HasDownloadsComboBox.SelectedIndex <0||
                ReleaseCountsTextBox?.Text == "" 
                    )
            {
                ResultLabel.Content = "Enter All Values";
                return;
            }
            List<string> LinesToWrite = new List<string>();
            LinesToWrite.Add("@relation \"GithubAll\"");
            LinesToWrite.Add("");
            LinesToWrite.Add("@attribute Language { PHP,JAVA,HTML,JavaScript,C}");
            LinesToWrite.Add("@attribute Contributers real");
            LinesToWrite.Add("@attribute Commits real");
            LinesToWrite.Add("@attribute Stars real");
            LinesToWrite.Add("@attribute Forks real");
            LinesToWrite.Add("@attribute Branches real");
            LinesToWrite.Add("@attribute Watchers real");
            LinesToWrite.Add("@attribute PullRequests real");
            LinesToWrite.Add("@attribute TotalIssues real");
            LinesToWrite.Add("@attribute OpenIssues real");
            LinesToWrite.Add("@attribute HasDownloads { TRUE,FALSE}");
            LinesToWrite.Add("@attribute ReleaseCount real");
            LinesToWrite.Add("@attribute isSuccessFull { TRUE,FALSE}");
            LinesToWrite.Add("");
            LinesToWrite.Add("@data");
            string Value = ((ComboBoxItem)LanguageComboBox.SelectedItem).Content.ToString();
            if (((ComboBoxItem)LanguageComboBox.SelectedItem).Content.ToString() == "C#/C++")
                Value = "C";
            LinesToWrite.Add(Value + "," +
                ContributersTextBox.Text + "," +
                CommitsTextBox.Text + "," +
                StarsTextBox.Text + "," +
                ForksTextBox.Text + "," +
                BranchesTextBox.Text + "," +
                WatchersTextBox.Text + "," +
                PullRequestsTextBox.Text + "," +
                TotalIssuesTextBox.Text + "," +
                OpenIssuesTextBox.Text + "," +
                ((ComboBoxItem)HasDownloadsComboBox.SelectedItem).Content.ToString() + "," +
                ReleaseCountsTextBox.Text + "," + "?"
                );
            System.IO.File.WriteAllLines("Dataset.arff", LinesToWrite);
            Classifier cl = SerializationHelper.read(@"RF.model") as Classifier;
            Instances testDataSet = new Instances(new java.io.FileReader("Dataset.arff"));
            testDataSet.setClassIndex(testDataSet.numAttributes() - 1);
            Evaluation evaluation = new Evaluation(testDataSet);

            for (int i = 0; i < testDataSet.numInstances(); i++)
            {
                Instance instance = testDataSet.instance(i);
                evaluation.evaluateModelOnceAndRecordPrediction(cl, instance);
            }
            double Prediction = -1;
            foreach (object o in evaluation.predictions().toArray())
            {
                NominalPrediction prediction = o as NominalPrediction;
                if (prediction != null)
                {
                    double[] distribution = prediction.distribution();
                    Prediction = prediction.predicted();
                }
            }
            if (Prediction == -1)
            {
                ResultLabel.Content = "Error Parsing";
            }else if (Prediction == 0)
            {
                ResultLabel.Content = "Successfull";
            }
            else
            {
                ResultLabel.Content = "UnSuccessfull";
            }
        }
    }
}
