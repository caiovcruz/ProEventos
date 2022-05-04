import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/services/evento.service';
import { environment } from '@environments/environment';
import { PaginatedResult, Pagination } from '@app/models/pagination';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss'],
})
export class EventoListaComponent implements OnInit {
  modalRef?: BsModalRef;
  public eventos: Evento[] = [];
  public eventoId: number = 0;
  public pagination = {} as Pagination;

  public larguraImagem = 150;
  public margemImagem = 2;
  public exibirImagem = true;

  public termChanged: Subject<string> = new Subject<string>();

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private router: Router
  ) {}

  public ngOnInit(): void {
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 1,
    } as Pagination;

    this.carregarEventos();
  }

  public alterarImagem(): void {
    this.exibirImagem = !this.exibirImagem;
  }

  public pathImagem(imagemURL: string): string {
    return imagemURL !== ''
      ? `${environment.apiURL + environment.pathImagesEvento + imagemURL}`
      : 'assets/img/semImagem.png';
  }

  public carregarEventos(term?: any): void {
    this.spinner.show();
    this.eventoService
      .getEventos(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        term
      )
      .subscribe(
        (response: PaginatedResult<Evento[]>) => {
          this.eventos = response.result;
          this.pagination = response.pagination;
        },
        (error: any) => {
          console.error(error);
          this.toastr.error('Erro ao carregar os Eventos.', 'Erro!');
        }
      )
      .add(() => this.spinner.hide());
  }

  public filtrarEventos(event: any): void {
    if (this.termChanged.observers.length === 0) {
      this.termChanged.pipe(debounceTime(1000)).subscribe((filterBy) => {
        this.carregarEventos(filterBy);
      });
    }
    this.termChanged.next(event.value);
  }

  public openModal(event: any, template: TemplateRef<any>, eventoId: number) {
    event.stopPropagation();
    this.eventoId = eventoId;
    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  public confirm(): void {
    this.modalRef?.hide();
    this.spinner.show();
    this.eventoService
      .delete(this.eventoId)
      .subscribe(
        (result: any) => {
          if (result.message === 'Deletado') {
            this.toastr.success(
              `O Evento #${this.eventoId} foi deletado com sucesso.`,
              'Deletado!'
            );
            this.carregarEventos();
          }
        },
        (error: any) => {
          console.error(error);
          this.toastr.error(
            `Erro ao deletar o Evento #${this.eventoId}.`,
            'Erro!'
          );
        }
      )
      .add(() => this.spinner.hide());
  }

  public decline(): void {
    this.modalRef?.hide();
  }

  public detalheEvento(id: number): void {
    this.router.navigate([`eventos/detalhe/${id}`]);
  }

  public pageChanged(event: PageChangedEvent): void {
    this.pagination.currentPage = event.page;
    this.carregarEventos();
  }
}
