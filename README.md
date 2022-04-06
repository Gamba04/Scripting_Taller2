# Taller 2

Integrantes:
Juan Camilo Quintero
Pedro Pablo Restrepo

Observaciones:
- Equipment se adiere a stats
- Se aplica el bono de affinity (+2, -2) a la hora de atacar, pero no modifica el stat como tal
- Como no estaba explicito en el enunciado, las skills de ReduceAP, ReduceRP y ReduceAll afectan a todas las cartas del Deck enemigo y RestoreRP afecta a todas las cartas del Deck propio. DestroyEquip debe especificar parametros opcionales de la carta Character a aplicar y el Equipo a destruir.
- Se utilizo herencia en las cartas partiendo de una clase base abstracta Card, ya que esta no debe ser instanciada como tal, con un constructor base protected que se hereda a los otros constructores.
- Se creo una clase Player para almacenar el Deck, ya que en el enunciado se mencionaba un "jugador" que contenia al Deck, pero la mayoria de funcionalidades parten del Deck en si
- Cuando una carta equip se aplica, esta se retira del Deck mediante un evento subscrito por el Deck previamente, cuando una carta SupportSkill se usa, esta se retira y cuando un Character muere, esta se retira de igual manera. Al destruir a un Character, se verifica si el Deck queda vacio, en caso tal el jugador correspondiente a ese Deck pierde.
- Se asumio que como un Character se considera muerto con 0 de vida, y se retira del Deck al morir, este no fuera valido para agregarse al Deck si tiene 0 de vida en primer lugar
