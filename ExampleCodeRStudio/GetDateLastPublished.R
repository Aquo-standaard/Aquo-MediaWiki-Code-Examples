GetDateLastPublished <- function(pDomeintabel) {
  library(jsonlite)
  library(httr)
  library(RCurl)
  
  maakTekstURL <- function(tekstURL,categorie,beperking,kenmerken,opmaak){
    returnstring <- paste(tekstURL,categorie,beperking,kenmerken,opmaak,sep="")
    return (returnstring)
  }
  
  bewerkDatum  <- function(pDatum){
    lDatum <- substring(toString(pDatum),3,nchar(pDatum)-2)
    lDatum <- toString(lubridate::parse_date_time(lDatum,orders="ymdHMS"))
    return(lDatum)
  }
  
  domeinTabel <- NULL
  tekstUrl <- "https://www.aquo.nl/index.php"
  lStartPage <- 0
  lLimit <- 1
  curl <- getCurlHandle()
  opmaakJson <- paste("%2F&format=json&link=none&headers=show&searchlabel=JSON&class=sortable+wikitable+smwtable"
                      ,"&theme=bootstrap&offset=0&limit=1"
                      ,"&mainlabel=&prettyprint=true&unescape=true"
                      ,sep="")
  
  categorie <- "?title=Speciaal:Vragen&x=-5B-5BElementtype%3A%3ADomeintabel%20%7C%7C%20Domeintabeltechnisch%20%7C%7C%20Domeintabelverzamellijst-5D-5D-20"
  kenmerken <- "%2F-3FElementtype%2F-3FVoorkeurslabel"
  beperking <- paste("-5B-5BVoorkeurslabel%3A%3A",pDomeintabel,sep = "")
  json_file <- maakTekstURL(tekstUrl,categorie,beperking,kenmerken,opmaakJson)
  #message(json_file)
  req <- httr::GET(json_file, curl=curl)
  
  lReturnDF <- data.frame(matrix(ncol = 3, nrow = 0))
  colnames(lReturnDF) <- as.list(c("Domeintabel","LaatstePublicatieDatum","Melding"))
  lReturnDF[1,"Domeintabel"] <- pDomeintabel
  
  if (req$status_code == 200 && length(req$content) > 0) {
    domeinTabel <- jsonlite::fromJSON(httr::content(req, "text", encoding="UTF-8"))$results
    
    domeinwaardeCategorie <- NULL
    domeinwaardeCategorie["Domeintabel"]              <- "Domeinwaarden"
    domeinwaardeCategorie["Domeintabeltechnisch"]     <- "DomeinwaardenTechnisch"
    domeinwaardeCategorie["Domeintabelverzamellijst"] <- "Domeinwaarden"
    
    lAantalDomTabellen <- length(domeinTabel)
    if (lAantalDomTabellen == 1) {
      domeinGuid <- domeinTabel[[1]]$fulltext
      domeinElementtype <- domeinTabel[[1]]$printouts$Elementtype$fulltext

      lTypeTabel <- paste("-5B-5BElementtype%3A%3A",domeinElementtype,"-5D-5D-20",sep = "")
      beperking <- paste("-5B-5BBreder%3A%3A",gsub("-","-2D",domeinGuid),"-5D-5D",sep = "")
      categorie <- paste("?title=Speciaal:Vragen&x=-5B-5BCategorie%3A",
                         domeinwaardeCategorie[domeinElementtype],"-5D-5D-20",sep = "")
      
      kenmerken <- "%2F-3FVoorkeurslabel%2F-3FWijzigingsdatum"
      opmaakJson <- paste("/format%3Djson/link%3Dall/headers%3Dshow/searchlabel=JSON/class=sortable-20wikitable-20smwtable"
                          ,"/sort%3DWijzigingsdatum/order%3Ddesc"
                          ,"/theme=bootstrap/offset=0/limit=1"
                          ,"/mainlabel=/prettyprint=true/unescape=true"
                          ,sep="")
      json_file <- maakTekstURL(tekstUrl,categorie,beperking,kenmerken,opmaakJson)
      #message(json_file)
      req <- httr::GET(json_file, curl=curl)
      if (req$status_code == 200 && length(req$content) > 0) {
        domainsJson <- jsonlite::fromJSON(httr::content(req, "text", encoding="UTF-8"))$results
        message(paste("Laatst gewijzigd:",unlist(domainsJson[[1]]$printouts["Voorkeurslabel"])))
        lDatum <- bewerkDatum(unlist(domainsJson[[1]]$printouts["Wijzigingsdatum"][[1]]$raw))
        lReturnDF[1,"LaatstePublicatieDatum"] <- lDatum
      }
      else lReturnDF[1,"Melding"] <- "Domeintabel heeft geen domeinwaarden."
    }
  }  
  else {
    lReturnDF[1,"Melding"] <- "Domeintabel bestaat niet."
  }
  return(lReturnDF)
}