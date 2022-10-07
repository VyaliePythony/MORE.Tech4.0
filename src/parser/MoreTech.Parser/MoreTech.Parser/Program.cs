using MoreTech.Parser;

var parser = new Parser();
await foreach (var result in parser.ParseRSS("https://www.vedomosti.ru/rss/rubric/business/sport"))
{

}

//"p.box-paragraph__text"