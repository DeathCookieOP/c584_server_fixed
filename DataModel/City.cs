using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataModel;

[Table("cities")]
public partial class City
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }

    [Column("name")]
    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("ascii")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Ascii { get; set; } = null!;

    [Column("lat", TypeName = "decimal(18, 4)")]
    public decimal Lat { get; set; }

    [Column("lag", TypeName = "decimal(18, 4)")]
    public decimal Lag { get; set; }

    [Column("population")]
    public int Population { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("Cities")]
    public virtual Country Country { get; set; } = null!;
}
