#!/usr/bin/python3

import sys
from  pathlib import Path

project_root_path = Path(__file__).absolute().parent.parent
docker_env_file_filename = str(project_root_path / 'env_collaction')
vscode_env_file_filename = str(project_root_path / 'CollAction' / '.env')

lines = []
try:
    with open(docker_env_file_filename, 'r') as file:
        lines = file.readlines()
except FileNotFoundError:
    print('File `{}` not found in this directory!'.format(docker_env_file_filename), file=sys.stderr)
    exit(1)

print('Creating `{}`...'.format(vscode_env_file_filename), end='')

with open(vscode_env_file_filename, 'w') as file:
    for line in lines:
        file.write(line.split('=')[0].replace(':', '__'))
        file.write('=')
        file.write(line.split('=')[1])

print('done')
