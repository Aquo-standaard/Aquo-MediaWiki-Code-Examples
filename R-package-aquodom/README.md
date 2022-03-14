
<!-- README.md is generated from README.Rmd. Please edit that file -->

# aquodom

<!-- badges: start -->

[![CRAN
status](https://www.r-pkg.org/badges/version/aquodom)](https://CRAN.R-project.org/package=aquodom)
[![R-CMD-check](https://github.com/RedTent/aquodom/workflows/R-CMD-check/badge.svg)](https://github.com/RedTent/aquodom/actions)
[![Lifecycle:
experimental](https://img.shields.io/badge/lifecycle-experimental-orange.svg)](https://lifecycle.r-lib.org/articles/stages.html#experimental)
<!-- badges: end -->

Met *aquodom* is het op eenvoudige wijze mogelijk om de
aquo-domeintabellen te downloaden en te gebruiken in R. De website voor
*aquodom* is te vinden via <https://redtent.github.io/aquodom>. 

## Installatie

*aquodom* is te installeren vanaf CRAN met:

``` r
install.packages("aquodom") 
```

De ontwikkelversie is te installeren van
[GitHub](https://github.com/RedTent/aquodom) met:

``` r
# install.packages("devtools")
devtools::install_github("RedTent/aquodom")
```

## Aquo-domeintabellen

De Aquo-standaard vormt de Nederlandse standaard voor de uitwisseling
van gegevens in het waterbeheer. Met *aquodom* (kort voor
aquo-domeintabellen) is het makkelijk om via de API domeintabellen van
de Aquo-standaard in R te downloaden en te gebruiken.

De belangrijkste functies van *aquodom* zijn `dom()` en `dom_save()`.
Met deze functies kan iedere domeintabel van www.aquo.nl worden
gedownload. De functie `dom()` geeft de domeintabel als een dataframe.
`dom_save()` doet hetzelfde maar slaat daarnaast ook de domeintabel op
als xlsx- of csv-bestand. De functie `dom_overzicht()` geeft een
compleet overzicht van alle beschikbare domeintabellen.

Alle functies hebben een optioneel argument `peildatum`. Dit argument
kan worden gebruikt om alleen domeinwaarden of domeintabellen te tonen
die geldig zijn op de peildatum. Met `peildatum = NULL` worden alle
resultaten inclusief verouderde waarden getoond.

``` r
library(aquodom)

dom("MonsterType")
#> # A tibble: 7 x 6
#>      id omschrijving   begin_geldigheid eind_geldigheid guid      gerelateerd   
#>   <dbl> <chr>          <date>           <date>          <chr>     <chr>         
#> 1    10 analysemonster 2017-12-13       2100-01-01      Id-6a3e6~ Id-99092d94-d~
#> 2     8 materiaalmons~ 2015-11-18       2100-01-01      Id-f811d~ Id-2d146a3e-3~
#> 3    11 samengevoegd ~ 2017-12-13       2100-01-01      Id-81ce3~ Id-8df42796-7~
#> 4     4 toetsmonster   2015-11-18       2100-01-01      Id-0034d~ Id-ad4f1180-6~
#> 5     9 uitloogmonster 2015-11-18       2100-01-01      Id-6053f~ Id-48826f74-c~
#> 6     1 veldmonster    2015-11-18       2100-01-01      Id-74dd8~ Id-3e9918e3-4~
#> 7     7 zeefmonster    2015-11-18       2100-01-01      Id-8d483~ Id-63ac95ff-1~

#De namen van domeintabellen zijn niet hoofdlettergevoelig
all.equal(dom("MonsterType"), dom("monstertype"))
#> [1] TRUE

# Alle domeinwaarden inclusief verouderde waarden
dom("MonsterType", peildatum = NULL)
#> # A tibble: 8 x 6
#>      id omschrijving   begin_geldigheid eind_geldigheid guid      gerelateerd   
#>   <dbl> <chr>          <date>           <date>          <chr>     <chr>         
#> 1    10 analysemonster 2017-12-13       2100-01-01      Id-6a3e6~ Id-99092d94-d~
#> 2     8 materiaalmons~ 2015-11-18       2100-01-01      Id-f811d~ Id-2d146a3e-3~
#> 3    10 mengmonster    2015-11-18       2017-12-12      Id-a0a78~ <NA>          
#> 4    11 samengevoegd ~ 2017-12-13       2100-01-01      Id-81ce3~ Id-8df42796-7~
#> 5     4 toetsmonster   2015-11-18       2100-01-01      Id-0034d~ Id-ad4f1180-6~
#> 6     9 uitloogmonster 2015-11-18       2100-01-01      Id-6053f~ Id-48826f74-c~
#> 7     1 veldmonster    2015-11-18       2100-01-01      Id-74dd8~ Id-3e9918e3-4~
#> 8     7 zeefmonster    2015-11-18       2100-01-01      Id-8d483~ Id-63ac95ff-1~

head(dom_overzicht(), 3)
#> # A tibble: 3 x 7
#>   domeintabel domeintabelsoort wijzigingsdatum begin_geldigheid eind_geldigheid
#>   <chr>       <chr>            <date>          <date>           <date>         
#> 1 Afsluitmid~ Domeintabel      2021-07-07      2016-03-12       2100-01-01     
#> 2 Bekleding_~ Domeintabel      2021-07-07      2016-03-12       2100-01-01     
#> 3 BekledingT~ Domeintabel      2021-07-07      2016-03-12       2100-01-01     
#> # ... with 2 more variables: kolommen <list>, guid <chr>

nrow(dom_overzicht())
#> [1] 132
# inclusief ongeldige domeintabellen
nrow(dom_overzicht(peildatum = NULL))
#> [1] 261
```

``` r

dom_save("monstertype")
```

## Caching en opslaan

Het downloaden van grotere domeintabellen kan behoorlijk wat tijd in
beslag nemen. Daarom maakt *aquodom* gebruik van caching. Als een
domeintabel eenmaal is gedownload wordt in dezelfde R-sessie gebruik
gemaakt van de cache. Voor het gebruik van dezelfde domeintabel in
verschillende R-sessies kan de domeintabel het beste opgeslagen worden,
bijv. met `dom_save()`.

``` r
# De eerste keer duurt vrij lang
system.time(dom("Hoedanigheid"))
#> ..
#>    user  system elapsed 
#>    2.05    1.02    6.22

# De tweede keer gaat veel sneller
system.time(dom("Hoedanigheid"))
#>    user  system elapsed 
#>    0.05    0.05    0.11
```
