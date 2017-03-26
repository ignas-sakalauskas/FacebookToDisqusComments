# FacebookToDisqusComments
Tool to migrate comments from Facebook to Disqus. The tool retrieves Facebook comments from OpenGraph API, formats them into Disqus comments XML format, and saves as separate XML files.

## Get started
In appsettings.json specify Facebook App ID and secret, output path, and input file. 
Input data format - one line per Facebook comments page, tab seperated values. Patter:
Comments_PageId[TAB]Target_Page_Title[TAB]Target_Page_URL[TAB]Target_Page_ID

For example:

11111	Page Title 1	https://ignas.me/page1	111

22222	Page Title 2	https://ignas.me/page2	222

## Blog post
A detailed blog post about using this tool for an actual comments migration.

https://ignas.me/tech/migrate-facebook-app-disqus-comments/ 
