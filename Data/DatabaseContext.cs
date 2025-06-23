using Fiap.Web.Aluno.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Web.Aluno.Data;

public class DatabaseContext : DbContext {
    //Propriedade para manipular a entidade de representante
    public DbSet<RepresentanteModel> Representantes { get; set; }

    //Propriedade para manipular a entidade cliente
    public DbSet<ClienteModel> Clientes { get; set; }


    // Método utilizado para a criação dos elementos no banco de dados
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RepresentanteModel>(entity => {
            //definindo um nome para a tabela
            entity.ToTable("Representantes");
            //definindo a chave primaria
            entity.HasKey(e => e.RepresentanteId);
            //tornando o nome obrigatório
            entity.Property(e => e.NomeRepresentante).IsRequired();
            //adicionando índice único ao CPF
            entity.HasIndex(e => e.Cpf).IsUnique();
        });

        modelBuilder.Entity<ClienteModel>(entity => {
            // Define o nome da tabela para 'Clientes'
            entity.ToTable("Clientes");
            entity.HasKey(e => e.ClienteId);
            entity.Property(e => e.Nome).IsRequired();
            entity.Property(e => e.Email).IsRequired();

            // Especifica o tipo de dado para DataNascimento
            entity.Property(e => e.DataNascimento).HasColumnType("date");
            entity.Property(e => e.Observacao).HasMaxLength(500);

            // Configuração da relação com RepresentanteModel
            // Define a relação de um para um com RepresentanteModel
            entity.HasOne(e => e.Representante)
                // Indica que um Representante pode ter muitos Clientes
                .WithMany()
                // Define a chave estrangeira
                .HasForeignKey(e => e.RepresentanteId)
                // Torna a chave estrangeira obrigatória
                .IsRequired();
        });
    }
    public DatabaseContext(DbContextOptions options) : base(options) {}
    protected DatabaseContext() {}
}
