lLastPublishedDate <- GetDateLastPublished(pDomeintabel <- "Parameter")
lLastPublishedDate <- GetDateLastPublished(pDomeintabel <- "Waarnemingssoort")
lLastPublishedDate <- GetDateLastPublished(pDomeintabel <- "xcgfs")

lDomTabel           <- GetDomainTable(pDomeintabel <- "Bemonsteringsapparaat")
lDomTabel           <- GetDomainTable(pDomeintabel <- "Bemonsteringsapparaat",pCheckDate <- "2011-01-01")
lDomTabel           <- GetDomainTable(pDomeintabel <- "Parameter",pCheckDate <- "2012-01-01")
lDomTabel           <- GetDomainTable(pDomeintabel <- "asdfasdf",pCheckDate <- "2012-01-01")

lDomTabelChanges    <- GetDomainTableChanges(pDomeintabel <- "Bemonsteringsapparaat",pCheckDate <- "2020-02-01")
lDomTabelChanges    <- GetDomainTableChanges(pDomeintabel <- "Parameter",pCheckDate <- "2021-02-01")

lDomTabelNames      <- GetDomainTableNames(pCheckDate <- "2012-01-01")
lDomTabelNames      <- GetDomainTableNames()

setwd("H:/SVN-Aquo/beheertools/Externe scripts")
write.table(lDomTabel, file = "DomTabel.csv", sep = ";", na = "", row.names = FALSE,fileEncoding = "UTF8")
write.table(lDomTabelChanges, file = "DomTabelChanges.csv", sep = ";", na = "", row.names = FALSE,fileEncoding = "UTF8")
write.table(lDomTabelNames, file = "DomTabelNames.csv", sep = ";", na = "", row.names = FALSE,fileEncoding = "UTF8")
