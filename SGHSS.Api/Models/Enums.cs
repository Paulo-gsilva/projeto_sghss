namespace SGHSS.Api.Models;

public enum PerfilUsuario { Paciente, ProfissionalSaude, Administrador }

public enum TipoConsulta { Presencial, Teleconsulta }

public enum StatusConsulta { Agendada, Cancelada, Concluida }

public enum StatusLeito { Livre, Ocupado, Manutencao }

public enum StatusInternacao { Ativa, Alta, Transferida }