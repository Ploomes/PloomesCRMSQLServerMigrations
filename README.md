# PloomesCRMSQLServerMigrations
 
## Estrutura

Os scripts serão executados na ordem abaixo:

### Scripts
Aqui vão todos os scripts em comum.
Todos os scripts que devem ser executados independente do shard devem estar aqui, na forma de migration, isto é, criada uma tabela X, caso queira adicionar uma coluna à esta tabela, deve ser criado um novo arquivo que faça essa ação, ou seja, o script de criação da tabela não irá conter essa coluna, porém, haverá um script que irá adicionar essa coluna posteriormente.

O Journal aqui é uma tabela no banco.

### Procedures
Aqui vão todas as procedures. 
Existe um template para a criação das procedures que consiste em primeiro excluir caso exista e depois inserir. Isso permite que alterações possam ser feitas diretamente na procedure, facilitando o code review do que foi alterado.
Alterações em procedures são feitas no arquivo de fato, sem o esquema de migrations, devido ao comportamento citado anteriormente.

O Journal aqui é vazio.


### Shard'n'
Aqui vão todos os scripts específicos por shard, como criação de índices, etc.

O Journal aqui é uma tabela no banco.

### Views
Essa pasta contém um script para atualizar as views, sempre será executado ao fim das execuções anteriores.

O Journal aqui é vazio.

## Comportamentos
O Journal ser vazio ou não impacta no controle das migrations. Por padrão é utilizado o Journal baseado na tabela SchemaVersions, a qual contém todos os scripts que já foram executados e a sua data de execução, não executando novamente um script que tenha o mesmo nome.
Quando temos um Journal vazio, o que acontece é que não há o controle do que foi executado ou não, ou seja, tudo será executado.
O Journal vazio utilizamos em Procedures e Views, pois queremos que sejam executados toda vez.
O Journal com o controle utilizamos em Scripts e em Shard'n', pois queremos ter o controle do que já foi executado ou não.

É importante lembrar que no Journal padrão, pelo fato de ter o controle na forma de migrations, quaisquer alterações que se deseja fazer deve ser realizada em um novo script, pois ele não estará na tabela do banco, sendo assim executado.

Todas as UpgradeEngines são criadas com WithTransactionAlwaysRollback.
Qualquer erro que ocorrer em um upgrade acontecerá o rollback.

## Relatório
Existe a possibilidade de gerar um Html contendo o que foi executado.
![rel](https://user-images.githubusercontent.com/42729316/175650384-cd7c24bf-4e27-4803-bcca-00587ab3253d.png)
Ps: Essa feature não está implementada.
