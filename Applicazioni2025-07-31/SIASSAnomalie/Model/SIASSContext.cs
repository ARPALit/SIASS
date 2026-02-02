/*
 * Nome del progetto: SIASS
 * Copyright (C) 2025 Agenzia regionale per la protezione dell'ambiente ligure
 *
 * Questo programma è software libero: puoi ridistribuirlo e/o modificarlo
 * secondo i termini della GNU Affero General Public License pubblicata dalla
 * Free Software Foundation, sia la versione 3 della licenza, sia (a tua scelta)
 * qualsiasi versione successiva.
 *
 * Questo programma è distribuito nella speranza che possa essere utile,
 * ma SENZA ALCUNA GARANZIA; senza nemmeno la garanzia implicita di
 * COMMERCIABILITÀ o IDONEITÀ PER UNO SCOPO PARTICOLARE. Vedi la
 * GNU Affero General Public License per ulteriori dettagli.
 *
 * Dovresti aver ricevuto una copia della GNU Affero General Public License
 * insieme a questo programma. In caso contrario, vedi <https://www.gnu.org/licenses/>.
*/

using Microsoft.EntityFrameworkCore;

namespace SIASSAnomalie.Model;

public partial class SIASSContext : DbContext
{
	public SIASSContext()
	{
	}

	public SIASSContext(DbContextOptions<SIASSContext> options)
		: base(options)
	{
	}

	public virtual DbSet<SIASS_STAZIONI> SIASS_STAZIONI { get; set; }

	public virtual DbSet<SIAS_ANOMALIE> SIAS_ANOMALIE { get; set; }

	public virtual DbSet<SIAS_CONTROLLI_ANOMALIE> SIAS_CONTROLLI_ANOMALIE { get; set; }

	public virtual DbSet<SIAS_GRANDEZZE_STAZIONE> SIAS_GRANDEZZE_STAZIONE { get; set; }

	public virtual DbSet<SIAS_MISURAZIONI> SIAS_MISURAZIONI { get; set; }

	public virtual DbSet<SIAS_STAZIONI_RETI> SIAS_STAZIONI_RETI { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.HasDefaultSchema("SIASS");

		modelBuilder.Entity<SIASS_STAZIONI>(entity =>
		{
			entity.HasKey(e => e.ID_STAZIONE).HasName("SIASS_STAZIONI_PK");

			entity.HasIndex(e => e.CODICE_IDENTIFICATIVO, "SIASS_STAZIONI_UK1");

			entity.Property(e => e.ID_STAZIONE)
				.ValueGeneratedOnAdd()
				.HasColumnType("NUMBER");
			entity.Property(e => e.ALLESTIMENTO)
				.HasMaxLength(50)
				.IsUnicode(false);
			entity.Property(e => e.ANNOTAZIONI)
				.HasMaxLength(1000)
				.IsUnicode(false);
			entity.Property(e => e.AUTORE_ULTIMO_AGGIORNAMENTO)
				.IsRequired()
				.HasMaxLength(50)
				.IsUnicode(false);
			entity.Property(e => e.CODICE_IDENTIFICATIVO)
				.IsRequired()
				.HasMaxLength(20)
				.IsUnicode(false)
				.ValueGeneratedOnAdd();
			entity.Property(e => e.CONTROLLO_ANOMALIE)
				.IsRequired()
				.HasPrecision(1)
				.HasDefaultValueSql("0 ");
			entity.Property(e => e.DESCRIZIONE)
				.IsRequired()
				.HasMaxLength(200)
				.IsUnicode(false);
			entity.Property(e => e.ESCLUSA_MONITORAGGIO)
				.IsRequired()
				.HasPrecision(1)
				.HasDefaultValueSql("0");
			entity.Property(e => e.ID_ALLEGATO_FOTO_STAZIONE).HasColumnType("NUMBER");
			entity.Property(e => e.ID_ALLEGATO_MAPPA).HasColumnType("NUMBER");
			entity.Property(e => e.ID_SITO).HasColumnType("NUMBER");
			entity.Property(e => e.ID_TIPO_STAZIONE).HasColumnType("NUMBER");
			entity.Property(e => e.TELETRASMISSIONE)
				.IsRequired()
				.HasPrecision(1)
				.HasDefaultValueSql("0 ");
			entity.Property(e => e.ULTIMO_AGGIORNAMENTO).HasColumnType("DATE");
		});

		modelBuilder.Entity<SIAS_ANOMALIE>(entity =>
		{
			entity.HasKey(e => e.ID_ANOMALIA).HasName("SIAS_ANOMALIE_PK");

			entity.Property(e => e.ID_ANOMALIA)
				.ValueGeneratedOnAdd()
				.HasColumnType("NUMBER");
			entity.Property(e => e.DATA_CONTROLLO).HasColumnType("DATE");
			entity.Property(e => e.DATA_VALORE).HasColumnType("DATE");
			entity.Property(e => e.ID_CONTROLLO).HasColumnType("NUMBER");
			entity.Property(e => e.ID_STAZIONE).HasColumnType("NUMBER");
			entity.Property(e => e.VALORE).HasColumnType("NUMBER");

			entity.HasOne(d => d.ID_CONTROLLONavigation).WithMany(p => p.SIAS_ANOMALIE)
				.HasForeignKey(d => d.ID_CONTROLLO)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("SIAS_ANOMALIE_FK2");

			entity.HasOne(d => d.ID_STAZIONENavigation).WithMany(p => p.SIAS_ANOMALIE)
				.HasForeignKey(d => d.ID_STAZIONE)
				.HasConstraintName("SIAS_ANOMALIE_FK1");
		});

		modelBuilder.Entity<SIAS_CONTROLLI_ANOMALIE>(entity =>
		{
			entity.HasKey(e => e.ID_CONTROLLO).HasName("SIAS_CONTROLLI_ANOMALIE_PK");

			entity.Property(e => e.ID_CONTROLLO)
				.ValueGeneratedOnAdd()
				.HasColumnType("NUMBER");
			entity.Property(e => e.ABILITATO)
				.IsRequired()
				.HasPrecision(1)
				.HasDefaultValueSql("0 ");
			entity.Property(e => e.CODICE_FUNZIONE)
				.IsRequired()
				.HasMaxLength(20)
				.IsUnicode(false);
			entity.Property(e => e.CONFIGURAZIONE_JSON)
				.IsRequired()
				.HasMaxLength(1000)
				.IsUnicode(false);
			entity.Property(e => e.DATA_ULTIMA_ESECUZIONE).HasColumnType("DATE");
			entity.Property(e => e.DESCRIZIONE_CONTROLLO)
				.HasMaxLength(1000)
				.IsUnicode(false);
			entity.Property(e => e.ESITO_ULTIMA_ESECUZIONE)
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.GRANDEZZA)
				.IsRequired()
				.HasMaxLength(50)
				.IsUnicode(false);
			entity.Property(e => e.ID_TIPO_STAZIONE).HasColumnType("NUMBER");
			entity.Property(e => e.NOME_CONTROLLO)
				.IsRequired()
				.HasMaxLength(50)
				.IsUnicode(false);
			entity.Property(e => e.NOME_UNITA_MISURA)
				.IsRequired()
				.HasMaxLength(50)
				.IsUnicode(false);
			entity.Property(e => e.RETE)
				.IsRequired()
				.HasMaxLength(100)
				.IsUnicode(false);
		});

		modelBuilder.Entity<SIAS_GRANDEZZE_STAZIONE>(entity =>
		{
			entity.HasKey(e => e.ID_GRANDEZZA_STAZIONE).HasName("SIAS_GRANDEZZE_STAZIONE_PK");

			entity.HasIndex(e => new { e.ID_STAZIONE, e.GRANDEZZA, e.UNITA_MISURA }, "SIAS_GRANDEZZE_STAZIONE_UK1").IsUnique();

			entity.Property(e => e.ID_GRANDEZZA_STAZIONE)
				.ValueGeneratedOnAdd()
				.HasColumnType("NUMBER");
			entity.Property(e => e.GRANDEZZA)
				.IsRequired()
				.HasMaxLength(50)
				.IsUnicode(false);
			entity.Property(e => e.ID_STAZIONE).HasColumnType("NUMBER");
			entity.Property(e => e.NUMERO_DECIMALI)
				.HasDefaultValueSql("0 ")
				.HasColumnType("NUMBER");
			entity.Property(e => e.UNITA_MISURA)
				.IsRequired()
				.HasMaxLength(50)
				.IsUnicode(false);

			entity.HasOne(d => d.ID_STAZIONENavigation).WithMany(p => p.SIAS_GRANDEZZE_STAZIONE)
				.HasForeignKey(d => d.ID_STAZIONE)
				.HasConstraintName("SIAS_GRANDEZZE_STAZIONE_FK3");
		});

		modelBuilder.Entity<SIAS_MISURAZIONI>(entity =>
		{
			entity.HasKey(e => e.ID_MISURAZIONE).HasName("SIAS_MISURAZIONI_PK");

			entity.HasIndex(e => new { e.DATA_MISURAZIONE, e.ID_GRANDEZZA_STAZIONE }, "SIAS_MISURAZIONI_INDEX1");

			entity.HasIndex(e => new { e.DATA_MISURAZIONE, e.ID_GRANDEZZA_STAZIONE, e.VALIDATA }, "SIAS_MISURAZIONI_INDEX2");

			entity.HasIndex(e => new { e.DATA_MISURAZIONE, e.ID_GRANDEZZA_STAZIONE, e.ID_INTERVENTO }, "SIAS_MISURAZIONI_INDEX3");

			entity.Property(e => e.ID_MISURAZIONE)
				.ValueGeneratedOnAdd()
				.HasColumnType("NUMBER");
			entity.Property(e => e.AUTORE_ULTIMO_AGGIORNAMENTO)
				.IsRequired()
				.HasMaxLength(50)
				.IsUnicode(false);
			entity.Property(e => e.CODICE_IDENTIFICATIVO_SENSORE)
				.HasMaxLength(50)
				.IsUnicode(false);
			entity.Property(e => e.DATA_MISURAZIONE).HasColumnType("DATE");
			entity.Property(e => e.FONTE_ARPAL)
				.IsRequired()
				.HasPrecision(1)
				.HasDefaultValueSql("1 ");
			entity.Property(e => e.ID_GRANDEZZA_STAZIONE).HasColumnType("NUMBER");
			entity.Property(e => e.ID_INTERVENTO).HasColumnType("NUMBER");
			entity.Property(e => e.ULTIMO_AGGIORNAMENTO).HasColumnType("DATE");
			entity.Property(e => e.VALIDATA)
				.HasDefaultValueSql("0")
				.HasColumnType("NUMBER");
			entity.Property(e => e.VALORE).HasColumnType("NUMBER");
			entity.Property(e => e.VALORE_SENSORE).HasColumnType("NUMBER");

			entity.HasOne(d => d.ID_GRANDEZZA_STAZIONENavigation).WithMany(p => p.SIAS_MISURAZIONI)
				.HasForeignKey(d => d.ID_GRANDEZZA_STAZIONE)
				.HasConstraintName("SIAS_MISURAZIONI_FK1");
		});

		modelBuilder.Entity<SIAS_STAZIONI_RETI>(entity =>
		{
			entity.HasKey(e => e.ID_STAZIONE_RETE).HasName("SIAS_STAZIONI_RETI_PK");

			entity.HasIndex(e => new { e.ID_STAZIONE, e.RETE }, "SIAS_STAZIONI_RETI_UK1").IsUnique();

			entity.Property(e => e.ID_STAZIONE_RETE)
				.ValueGeneratedOnAdd()
				.HasColumnType("NUMBER");
			entity.Property(e => e.ID_STAZIONE).HasColumnType("NUMBER");
			entity.Property(e => e.RETE)
				.IsRequired()
				.HasMaxLength(100)
				.IsUnicode(false);

			entity.HasOne(d => d.ID_STAZIONENavigation).WithMany(p => p.SIAS_STAZIONI_RETI)
				.HasForeignKey(d => d.ID_STAZIONE)
				.HasConstraintName("SIAS_STAZIONI_RETI_FK1");
		});
		modelBuilder.HasSequence("SIAS_ALLEGATI_INTERVENTI_SEQ");
		modelBuilder.HasSequence("SIAS_ANOMALIE_SEQ");
		modelBuilder.HasSequence("SIAS_CONTROLLI_ANOMALIE_SEQ");
		modelBuilder.HasSequence("SIAS_FINALITA_STAZIONI_SEQ");
		modelBuilder.HasSequence("SIAS_GRANDEZZE_STAZIONE_SEQ");
		modelBuilder.HasSequence("SIAS_IMPORT_ORGANOCLORUR_SEQ");
		modelBuilder.HasSequence("SIAS_INTERVENTI_SEQ");
		modelBuilder.HasSequence("SIAS_MISURAZIONI_SEQ");
		modelBuilder.HasSequence("SIAS_OPERATORI_INTERV_SUP_SEQ");
		modelBuilder.HasSequence("SIAS_OPERATORI_INTERVENTI_SEQ");
		modelBuilder.HasSequence("SIAS_ORGANOCLORURATI_SEQ");
		modelBuilder.HasSequence("SIAS_PACCHETTI_INTERVENTI_SEQ");
		modelBuilder.HasSequence("SIAS_PACCHETTI_STRUMENTI_SEQ");
		modelBuilder.HasSequence("SIAS_SITI_SEQ");
		modelBuilder.HasSequence("SIAS_STAZIONI_RETI_SEQ");
		modelBuilder.HasSequence("SIAS_STRUMENTI_STAZIONE_SEQ");
		modelBuilder.HasSequence("SIASS_ALLEGATI_INTERVENTI_SEQ");
		modelBuilder.HasSequence("SIASS_ALLEGATI_STAZIONI_SEQ");
		modelBuilder.HasSequence("SIASS_CARATT_CENTRALINE_SEQ");
		modelBuilder.HasSequence("SIASS_CARATT_INSTALLAZIONI_SEQ");
		modelBuilder.HasSequence("SIASS_CARATT_TECN_POZZI_SEQ");
		modelBuilder.HasSequence("SIASS_DATI_AMMINISTRATIVI_SEQ");
		modelBuilder.HasSequence("SIASS_INTERVENTI_LOG_SEQ");
		modelBuilder.HasSequence("SIASS_INTERVENTI_SEQ");
		modelBuilder.HasSequence("SIASS_LOCALIZZAZIONI_SEQ");
		modelBuilder.HasSequence("SIASS_MISURAZIONI_SEQ");
		modelBuilder.HasSequence("SIASS_SENSORI_SEQ");
		modelBuilder.HasSequence("SIASS_STAZIONI_LOG_SEQ");
		modelBuilder.HasSequence("SIASS_STAZIONI_SEQ");

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
