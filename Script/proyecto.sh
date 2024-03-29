#!/bin/bash

#si no se introduce ningún parámetro proceder con el índice de opciones válidas
if [ $# -eq 0 ]; then
echo "Este script presenta múltiples opciones, escriba la opción de ejecución deseada de la lista que se facilita a continuacón: "
echo "add: Permite añadir archivos .txt a la carpeta Content del Moogle para que se puedan utilizar en el programa."
echo "run: Ejecuta el Moogle."
echo "report: Genera el informe.pdf del Moogle."
echo "slides: Genenra la presentacion.pdf del Moogle."
echo "show_report: Muestra el informe.pdf del Moogle y lo genera si no ha sido hecho con anterioridad."
echo "show_slides: Muestra la presentacion.pdf del Moogle y lo genera si no ha sido hecho con anterioridad."
echo "clean: Elimina los archivos auxiliares generados durante la ejecución de los .tex."
echo "crash: Cierra el script sin necesidad de cerrar la terminal."

read action
trap ctrl_c SIGINT

#en caso de que al ejecutar ./proyecto.sh se incluyera una ación detrás, ese parametro da valor a la variable para comenzar con el ciclo.
else
action="$1"
trap ctrl_c SIGINT
fi

#obtener la ruta absoluta del directorio padre de la carpeta Script en la que se encuentra el .sh
#como las carpetas Script, Informe, Presentación y Content son hermanas todas necesitan pasar por aquí para poder trabajar con ellas
parent_dir=$(cd .. && pwd)

while [[ "$action" != "crash" ]]
do

    #optener la ruta absoluta del directorio actual
    current_dir=$(pwd)

    case $action in 

        #ejcuta el moogle
        run)
            #Este pedazo de código comprueba si la ruta padre del script y la ruta actual son la misma, para no salirse de la carpeta
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi 
            make dev 
        ;;

        add)
            #ruta completa de la carpeta Content que es donde se agregan los .txt 
            content="$parent_dir/Content"
            read -p "Introduzca la ruta de la carpeta que contiene los archivos .txt que quiere agregar: " ruta_carpeta

            #verificar si la ruta de la carpeta es válida
            if [ -d "$ruta_carpeta" ]; then

                echo "Selecciona una opción: "

                select opcion in "Añadir un archivo" "Añadir todos"; do
                    
                    if [ -n "$opcion" ]; then
                        
                        case $opcion in

                            "Añadir un archivo")
                                read -p "Introduzca el nombre del archivo .txt que desea añadir: " archivo
                                cp "$ruta_carpeta/$archivo.txt" "$content/"
                                break
                            ;;

                            "Añadir todos")
                                cp "$ruta_carpeta/"*.txt"" "$content/"
                                echo "Archivos agregados."
                                break
                            ;;
                        
                        esac
                    
                    else
                        echo "Opción inválida."
                    
                    fi
                    break
                
                done
            else
                echo "La ruta de la carpeta no es válida."
            fi

        ;;

        #Genera el pdf del informe y los archivos auxiliares que vienen con él
        report)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Informe
            pdflatex informe.tex
        ;;

        #Genera el pdf de la presentación y los archivos auxiliares que vienen con él
        slides)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Presentación
            pdflatex presentacion.tex
        ;;

        #Muestra el PDF del informe y lo genera en caso de no haber sido generado anteriormente
        show_report)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Informe

            #se consulta si el pdf ya existe para que no tenga que generar el .tex innecesariamente
            if [ -z "$(find *.pdf)" ]; then
                pdflatex informe.tex
            fi 

            #se le da la opción al usuario de elegir su propio visualizador de pdf o el predeterminado
            read -p "Introduzca el visualizador de pdf de su preferencia o 1 para el visualizador predeterminado: " visulizador

            #En caso de que se escoja el predeterminado, se comprueba q esté instalado y si es así se ejecuta.
            #si no está, se pide que ingrese el comando de uno q si exista y ese es el que se ejecuta.
            if [ "$visulizador" = "1" ]; then

                if dpkg-query -W -f='${Status}' evince 2>/dev/null | grep -q "^install ok installed$"; then
                    evince informe.pdf &
                else
                    read -p "El visulizador predeterminado no está instalado en el sistema, por favor introduzca uno propio: " visualizer
                    $visualizer informe.pdf &
                fi

            #En caso de introducirse un visualizador propio directamente se ejecuta con él
            else
                $visulizador informe.pdf &
            fi
        ;;
        
        #Muestra el PDF de la presentación y lo genera en caso de no haber sido generado anteriormente
        show_slides)
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Presentación

            #se consulta si el pdf ya existe para que no tenga que generar el .tex innecesariamente
            if [ -z "$(find *.pdf)" ]; then
                pdflatex presentacion.tex
            fi 

            #se le da la opción al usuario de elegir su propio visualizador de pdf o el predeterminado
            read -p "Introduzca el visualizador de pdf de su preferencia o 1 para el visualizador predeterminado: " visulizador

            #En caso de que se escoja el predeterminado, se comprueba q este instalado y si es así se ejecuta.
            #si no esta, se pide q ingrese en comando de uno q si exista y ese es el que se ejecuta.
            if [ "$visulizador" = "1" ]; then

                if dpkg-query -W -f='${Status}' evince 2>/dev/null | grep -q "^install ok installed$"; then
                    evince presentacion.pdf &
                else
                    read -p "El visulizador predeterminado no está instalado en el sistema, por favor introduzca uno propio: " visualizer
                    $visualizer presentacion.pdf &
                fi

            #En caso de introducirse un visualizador propio directamente se ejecuta con él
            else
                $visulizador presentacion.pdf &
            fi
        ;;

        #Si se encuentran los archivos auxiliares generados por el .tex y el proyecto los elimina
        clean)
        #aquí elimina los del informe.tex
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Informe
            archivo1=$(find *.aux)
            if [ -n "$archivo1" ]; then
                rm -v *.aux
                rm -v *.log
                rm -v *.pdf
            else
                echo "No hay archivos que borrar generados por informe.tex"
            fi

        #aquí elimina los de la presentacion.tex
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi
            cd Presentación
            archivo1=$(find *.aux)
            if [ -n "$archivo1" ]; then
                rm -v *.aux
                rm -v *.log
                rm -v *.pdf
                rm -v *.nav
                rm -v *.out
                rm -v *.snm
                rm -v *.toc
            else
                echo "No hay archivos que borrar generados por presentacion.tex"
            fi

        #Aquí elimina los del proyecto
            if [ "$current_dir" != "$parent_dir" ]; then
                cd ..
            fi

            archivo1=$(find MoogleEngine/bin)
            archivo2=$(find MoogleServer/bin)
            if [ -n "$archivo1" ]; then
                rm -rv MoogleEngine/bin MoogleEngine/obj
            elif [ -n "$archivo2" ]; then
                rm -rv MoogleServer/bin MoogleServer/obj
            else
                echo "No hay archivos que borrar generados por el proyecto"
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

    echo "Este script presenta múltiples opciones, escriba la opción de ejecución deseada de la lista que se facilita a continuacón: "
    echo "add: Permite añadir archivos .txt a la carpeta Content del Moogle para que se puedan utilizar en el programa."
    echo "run: Ejecuta el Moogle."
    echo "report: Genera el informe.pdf del Moogle."
    echo "slides: Genenra la presentacion.pdf del Moogle."
    echo "show_report: Muestra el informe.pdf del Moogle y lo genera si no ha sido hecho con anterioridad."
    echo "show_slides: Muestra la presentacion.pdf del Moogle y lo genera si no ha sido hecho con anterioridad."
    echo "clean: Elimina los archivos auxiliares generados durante la ejecución de los .tex."
    echo "crash: Cierra el script sin necesidad de cerrar la terminal."

    read action

done

#Se utiliza para evitar que se cierre el script cuando se cierra el moogle luego del run
ctrol_c() 
{
    echo "Se ha presionado Ctrl+C, el proyecto no se cerrará"
}