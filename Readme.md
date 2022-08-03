# MovieRecommender

Este proyecto intenta simular un recomendador de películas basado en similitud de usuarios: si dos usuarios puntúan las mismas películas de manera similar, estos se consideran similares y se recomiendan mutuamente sus películas favoritas.

El recomendador puede usarse del mismo modo pero de manera inversa: si dos usuarios votan las mismas películas de manera totalmente opuestas, estos usuarios son similares de manera inversa y, por lo tanto, pueden recomendarse mutuamente sus películas menos favoritas.

### Similitud

La similitud entre dos usuarios viene definida por la suma en valor absoluto de las diferencias de puntuación en las películas vistas por ambos usuarios. Cuanto más cercana a cero sea esta suma, más similares serán los usuarios.

Sean _x<sub>1</sub>, x<sub>2</sub>, ..., x<sub>N</sub>_ las puntuaciones del usuario _X_ a las películas _P<sub>1</sub>,P<sub>2</sub>...,P<sub>N</sub>_ y sean _y<sub>1</sub>, y<sub>2</sub>, ..., y<sub>N</sub>_ las puntuaciones del usuario _Y_ a las mismas películas. Entonces, se tiene que:

_0 <= |x<sub>1</sub> - y<sub>1</sub>| + |x<sub>2</sub> - y<sub>2</sub>| + ... + |x<sub>N</sub> - y<sub>N</sub>| <= (P<sub>Max</sub> - P<sub>Min</sub>) x N_

donde _P<sub>Max</sub>_ y _P<sub>Min</sub>_ son las puntuaciones máximas y mínimas que se le puede dar a una película.

Teniendo esto en cuenta, definiremos la afinidad de los usuarios _X_, _Y_ como:

_A<sub>X,Y</sub> = ((P<sub>Max</sub> - P<sub>Min</sub>) x N - D<sub>X,Y</sub>) / (P<sub>Max</sub> - P<sub>Min</sub>) x N_

donde _D<sub>X,Y</sub> = |x<sub>1</sub> - y<sub>1</sub>| + |x<sub>2</sub> - y<sub>2</sub>| + ... + |x<sub>N</sub> - y<sub>N</sub>|_ y se tiene que: _0 <= A<sub>X,Y</sub> <= 1_.

Así, dos usuarios se considerarán similares si su afinidad es superior o igual al porcentaje de afinidad definido en la creación de _RatingRepository_ (actualmente establecido en 0.8).

### Datos

El recomendador se ha implementado asumiendo que se disponen de 3 ficheros _.csv_ que contienen toda la información necesaria para inicializar los 3 repositorios siguientes:

- <u>Users</u>: donde se tendrá la información relativa a los usuarios a través de las columnas
    * Identificación (Id)
    * Nombre
    * Correo electrónico
    * Fecha de nacimiento
- <u>Movies</u>: donde se tendrá la información relativa a las películas a través de las columnas
    * Identificación (Id)
    * Título
    * Director
    * Género
- <u>Ratings</u>: dedicado a almacenar las valoraciones que hacen los usuarios de las películas a través de las columnas
    * Id película
    * Id usuario
    * Valoración
    
### Implementación

Una vez que el recomendador encuentre el usuario más similar (usuario _Y_) al que se le quiera recomendar una película (usuario _X_), se decidirá qué película favorita de _Y_, que no ha visto _X,_ se le recomienda teniendo en cuenta las preferencias de género de _X_. Para determinar las preferencias de género se ha asumido que la base de datos de películas es suficientemente diversa como para deducir que el usuario tiene predilección por el género más visto (y, por lo tanto, más veces valorado), sin tener en cuenta la valoración que haya dado a dichas películas.

Nota: Véase que podría aplicarse lo mismo teniendo en cuenta los directores de las películas en lugar de los géneros y asumir así que los directores más veces valorados son los preferidos por el usuario. La implementación sería exactamente la misma que la aplicada en este proyecto para los géneros.
