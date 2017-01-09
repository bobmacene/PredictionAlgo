# PredictionAlgo

PredictionAlgo Explained:
This is an ASP.NET MVC web application which uses an algorithm to predict results of Pro12 rugby matches. It also compares these predictions against bookmaker match spreads, and recommends teams to back, based on these comparisons. It shows previous prediction data and historical fixture data.

Technology/Frameworks/Tools used:
The web app was written in Visual Studio 2015 and is hosted in Azure, as is the web app’s database and database server. For source control GitHub was used with code being transferred to GitHub using the VS2015 team explorer extension. Azure continuous deployment was also used which automatically deploys new code submitted to GitHub to the Azure web app.

NUnit framework was used for unit testing, JetBrains Cover framework was used for code coverage review and VS2015 was used for code analysis. Microsoft SQL Server Management Server was used for database schema diagrams and table design screenshots. It was also used for data and table manipulation. However, it proved easier to build a console app in VS2015 to manipulate historical database data. This was done locally and was not submitted to GitHub or included as part of the project as it is not integral/necessary for the finished web app.
The web app uses CRUD operations on the Predict Results page to view future predictions of to see historical predictions.

The web app uses a webscraper which uses the HttpAgility framework to be effective. The scraper scrapes data from the PaddyPower website for betting data and the Pro12 website for results information.
