from os import listdir
from os.path import isfile, join

onlyfiles = [ f for f in listdir('.') if isfile(join('.',f)) and f.endswith('.sql')  ]

target = open('output.txt', 'w')

for f in onlyfiles:
    content = f.replace("_", "{\\textunderscore}")+"\n\\begin{lstlisting} \n"
    with open(f, 'r') as content_file:
        content += content_file.read().replace("_", "{\\textunderscore}")
    content += "\n\end{lstlisting}\n\n"
    target.write(content)
target.close()
