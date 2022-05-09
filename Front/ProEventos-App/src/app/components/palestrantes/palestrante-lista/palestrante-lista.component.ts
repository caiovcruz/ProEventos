import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PaginatedResult, Pagination } from '@app/models/pagination';
import { Palestrante } from '@app/models/Palestrante';
import { PalestranteService } from '@app/services/palestrante.service';
import { environment } from '@environments/environment';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-palestrante-lista',
  templateUrl: './palestrante-lista.component.html',
  styleUrls: ['./palestrante-lista.component.scss'],
})
export class PalestranteListaComponent implements OnInit {
  public palestrantes: Palestrante[] = [];
  public palestranteId: number = 0;
  public pagination = {} as Pagination;

  public termChanged: Subject<string> = new Subject<string>();

  constructor(
    private palestranteService: PalestranteService,
    private modalService: BsModalService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private router: Router
  ) {}

  ngOnInit() {
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 1,
    } as Pagination;

    this.carregarPalestrantes();
  }

  public carregarPalestrantes(term?: any): void {
    this.spinner.show();
    this.palestranteService
      .getPalestrantes(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        term
      )
      .subscribe(
        (response: PaginatedResult<Palestrante[]>) => {
          this.palestrantes = response.result;
          this.pagination = response.pagination;
        },
        (error: any) => {
          console.error(error);
          this.toastr.error('Erro ao carregar os Palestrantes.', 'Erro!');
        }
      )
      .add(() => this.spinner.hide());
  }

  public filtrarPalestrantes(event: any): void {
    if (this.termChanged.observers.length === 0) {
      this.termChanged.pipe(debounceTime(1000)).subscribe((filterBy) => {
        this.carregarPalestrantes(filterBy);
      });
    }
    this.termChanged.next(event.value);
  }

  public getImagemURL(imageName: string): string {
    return imageName
      ? `${environment.apiURL + environment.pathImagesPerfil + imageName}`
      : 'assets/img/semImagemPerfil.png';
  }
}
