from os import listdir
from os.path import isfile, join

dirtory = './store procedures/'

onlyfiles = [ f for f in listdir(dirtory) if isfile(join(dirtory,f)) and f.endswith('.sql')  ]

target = open('output.txt', 'w')

for f in onlyfiles:
    content = f.replace("_", "{\\textunderscore}")+"\n\\begin{lstlisting} \n"
    with open(dirtory+f, 'r') as content_file:
        content += content_file.read()
    content += "\n\end{lstlisting}\n\n"
    target.write(content)
target.close()
