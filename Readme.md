# Moogle!
Su función es devolver el título del documento que ha tenido una coincidencia con el query y un fragmento de este en el cual aparezca la frase o palabra buscada. 

Para ejecutarlo correctamente, añade documentos .txt en la carpeta Content y no le modifiques el nombre, utiliza el comando make dev y permite que cargue.

También se puede ejecutar mediante proyecto.sh de /Script, que permite no solo implementarlo sino agregar de manera dinámica los documentos a /Content, eliminar archivos secundarios innecesarios y generar/mostrar el informe y la presentación.

El tiempo de espera para su ejecución dependerá de la cantidad de documentos que haya agregado a la carpeta Content, pues antes de arrancar se analizan todos los textos para buscar una lista de palabras únicas generales, la creación de las tablas de tf e idf y finalmente de la tabla de tf por idf que será la que se utilizará para hallar la relevancia del documento. Una vez mostrada la interfaz gráfica todo lo que el usuario ha de hacer es introducir una frase o palabra para que se busquen los documentos que presenten coincidencias; este proceso se lleva a cabo por la vectorization del query y su cálculo de similitud de cosenos con el tf por idf de cada documento.