#!/bin/bash

if [ $# -eq 0 ]; then
echo "Este script presenta múltiples opciones, escriba la opción de ejecución deseada de la lista que se facilita a continuacón:
run: Ejecuta el Moogle
report: Genera el informe.pdf del Moogle
slides: Genenra la presentacion.pdf del Moogle
show_report: Muestra el informe.pdf del Moogle y lo genera si no ha sido hecho con anterioridad
show_slides: Muestra la presentacion.pdf del Moogle y lo genera si no ha sido hecho con anterioridad
clean: Elimina los archivos innecisarios generados durante la ejecución de los .tex
crash: Cierra el script sin necesidad de cerrar la terminal"

read opcion
trap ctrl_c SIGINT

else
opcion="$1"
trap ctrl_c SIGINT
fi

#obtener la ruta absoluta del directorio padre de la carpeta Script en la que se encuentra el .sh
parent_dir=$(cd .. && pwd)

while [[ "$opcion" != "crash" ]]
do

    #optener la ruta absoluta del directorio actual
    current_dir=$(pwd)

    case $opcion in 

        #ejcuta el moogle
        run)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi 
            make dev 
        ;;

        #Genera el pdf del informe y los archivos secundarios que vienen con él
        report)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Informe
            pdflatex informe.tex
        ;;

        #Genera el pdf de la presentación y los archivos secundarios que vienen con él
        slides)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Presentación
            pdflatex presentacion.tex
        ;;

        #Genera el pdf del informe y los archivos secundarios que vienen con él y muestra el pdf
        show_report)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Informe
            pdflatex informe.tex
            evince informe.pdf &
        ;;
        
        #Genera el pdf de la presentación y los archivos secundarios que vienen con él y muestra el pdf
        show_slides)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Presentación
            pdflatex presentacion.tex
            evince presentacion.pdf &
        ;;

        #Si se encuentran los archivos secundarios generados por el .tex los elimina, no se incluye el .pdf dentro de los eliminados
        clean)
        #aquí elimina los del informe.tex
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Informe
            archivo=$(find *.aux)
            if [ -n "$archivo" ]; then
                rm *.aux
                rm *.log
            else
            echo "No hay archivos que borrar generados por informe.tex"
            fi

        #aquí elimina los de la presentacion.tex
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Presentación
            archivo=$(find *.aux)
            if [ -n "$archivo" ]; then
                rm *.aux
                rm *.log
                rm *.nav
                rm *.out
                rm *.snm
                rm *.toc
            else
            echo "No hay archivos que borrar generados por presentacion.tex"
            fi
        ;;

        #cierra el script para poder seguir trabajando en la misma terminal/consola
        crash)
            exit 0
        ;;
        
        *)
            echo "Error: comando no encontrado."
        ;;

    esac

echo "Este script presenta múltiples opciones, escriba la opción de ejecución deseada de la lista que se facilita a continuacón:
run  
report  
slides  
show_report 
show_slides
clean
crash"

read opcion

done

#Se utiliza para evitar que se cierre el script cuando se cierra el moogle luego del run
ctrol_c() 
{
    echo "Se ha presionado Ctrl+C, el protecto no se cerrará"
}