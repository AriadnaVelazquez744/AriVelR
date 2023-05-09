# Moogle!
Su función es devolver el título del documento con el cual ha habido una coincidencia con el query y un fracmento de este documento en el cual aparezca la frase o palabra buscada. 

Para ejecutarlo correctamente, añade documentos .txt en la carpeta Content y no le modifiques el nombre, utiliza el comando make dev y permite que cargue.

El tiempo de espera para su ejecucion dependerá de la cantidad de documentos que haya agregado a la carpeta content, pues antes de arrancar se analizan todos los textos para buscar una lista de palabras únicas generales, la creacion de las tablas de tf e idf y finalmente de la tabla de tf*idf que será la que se utilizará para hallar la importancia del docuento. Una vez mostrada la interfaz gráfica todo lo que el usuario ha de hacer es introducir una frase o palabra para que se busquen los documentos que presenten coincidencias; este proceso se lleva a cabo por la vectorizacion del query y su calculo de similitud de cosenos con el tf*idf de cada documento.

Este buscador es altamente innovador y único pues no solo implementa codigos en C# para los algoritmos de búsqueda, sino que emplea el algebra lineal y calcula matrices y vectores. 
##A que es asombroso?! Pruébenlo!!!

