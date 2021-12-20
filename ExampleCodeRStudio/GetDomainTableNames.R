GetDomainTableNames <- function(pCheckDate=toString(Sys.Date())) {
  message(paste("pCheckDate",pCheckDate))
  library(jsonlite)
  library(httr)
  library(RCurl)
  
  maakTekstURL <- function(tekstURL,categorie,beperking,kenmerken,opmaak){
    returnstring <- paste(tekstURL,categorie,beperking,kenmerken,opmaak,sep="")
    return (returnstring)
  }
  
  bewerkDatum  <- function(pDatum){
    lDatum <- substring(toString(pDatum),3,nchar(pDatum))
    lDatum <- str_replace(lDatum,"/0/0/0/0","")
    lDatum <- toString(lubridate::parse_date_time(lDatum,orders="ymd"))
    return(lDatum)
  }
  
  domeinTabellen <- data.frame(matrix(ncol = 1, nrow = 0))
  colnames(domeinTabellen) <- as.list("Domeintabel")

  tekstUrl <- "https://www.aquo.nl/index.php"
  lStartPage <- 0
  lLimit <- 1
  curl <- getCurlHandle()
  opmaakJson <- paste("%2F&format=json&link=none&headers=show&searchlabel=JSON&class=sortable+wikitable+smwtable"
                      ,"&theme=bootstrap&offset=0&limit=500"
                      ,"&mainlabel=&prettyprint=true&unescape=true"
                      ,sep="")
  
  categorie <- "?title=Speciaal:Vragen&x=-5B-5BElementtype%3A%3ADomeintabel%20%7C%7C%20Domeintabeltechnisch%20%7C%7C%20Domeintabelverzamellijst-5D-5D-20"
  kenmerken <- "%2F-3FElementtype%2F-3FVoorkeurslabel%2F-3FMetadata"
  beperking <- paste("-5B-5BBegin-20geldigheid::<=",pCheckDate,"-5D-5D-5B-5BEind-20geldigheid::>=",pCheckDate,"-5D-5D",sep = "")
  json_file <- maakTekstURL(tekstUrl,categorie,beperking,kenmerken,opmaakJson)
  #message(json_file)
  req <- httr::GET(json_file, curl=curl)
  
  if (req$status_code == 200 && length(req$content) > 0) {
    domeinTabellenJson <- jsonlite::fromJSON(httr::content(req, "text", encoding="UTF-8"))$results
    
    for (di in 1:length(domeinTabellenJson))
        domeinTabellen[di,"Domeintabel"] <- domeinTabellenJson[[di]]$printouts$Voorkeurslabel

  }
  else message("Geen domeintabellen aanwezig")
  return(domeinTabellen)
}