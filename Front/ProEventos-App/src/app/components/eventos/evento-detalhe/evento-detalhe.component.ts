import { Component, OnInit, TemplateRef } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { FormAction } from '@app/enums/FormAction.enum';
import { ValidatorForm } from '@app/helpers/ValidatorForm';
import { Evento } from '@app/models/Evento';
import { Lote } from '@app/models/Lote';
import { EventoService } from '@app/services/evento.service';
import { LoteService } from '@app/services/lote.service';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss'],
})
export class EventoDetalheComponent implements OnInit {
  modalRef?: BsModalRef;
  eventoId!: number;
  evento = {} as Evento;
  form!: FormGroup;
  formAction = FormAction.Post;
  loteAtual = { id: 0, nome: '', index: 0 };

  get f(): any {
    return this.form.controls;
  }

  get modoEditar(): boolean {
    return this.formAction === FormAction.Put;
  }

  get lotes(): FormArray {
    return this.form.get('lotes') as FormArray;
  }

  get bsConfig(): any {
    return {
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY hh:mm A',
      containerClass: 'theme-default',
      showWeekNumbers: false,
    };
  }

  get bsConfigLote(): any {
    return {
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY',
      containerClass: 'theme-default',
      showWeekNumbers: false,
    };
  }

  constructor(
    private fb: FormBuilder,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private localeService: BsLocaleService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private eventoService: EventoService,
    private loteService: LoteService,
    private modalService: BsModalService
  ) {
    this.localeService.use('pt-br');
  }

  public carregarEvento(): void {
    this.eventoId = +this.activatedRoute.snapshot.paramMap.get('id')!;

    if (this.eventoId !== null && this.eventoId !== 0) {
      this.spinner.show();

      this.formAction = FormAction.Put;
      this.eventoService
        .getEventoById(this.eventoId)
        .subscribe(
          (evento: Evento) => {
            this.evento = { ...evento };
            this.form.patchValue(this.evento);
            this.carregarLotes();
          },
          (error: any) => {
            console.error(error);
            this.toastr.error('Erro ao carregar o Evento.', 'Erro!');
          }
        )
        .add(() => this.spinner.hide());
    }
  }

  public carregarLotes(): void {
    if (this.eventoId !== null && this.eventoId !== 0) {
      this.loteService
        .getLotesByEventoId(this.eventoId)
        .subscribe(
          (lotesEvento: Lote[]) => {
            lotesEvento.forEach((lote) => {
              this.lotes.push(this.criarLote(lote));
            });
          },
          (error: any) => {
            console.error(error);
            this.toastr.error('Erro ao carregar os Lotes.', 'Erro!');
          }
        )
        .add(() => this.spinner.hide());
    }
  }

  ngOnInit(): void {
    this.carregarEvento();
    this.validation();
  }

  public validation(): void {
    this.form = this.fb.group({
      tema: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(50),
        ],
      ],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      imagemURL: ['', Validators.required],
      lotes: this.fb.array([]),
    });
  }

  public adicionarLote(): void {
    this.lotes.push(this.criarLote({ id: 0 } as Lote));
  }

  public criarLote(lote: Lote): FormGroup {
    return this.fb.group({
      id: [lote.id],
      nome: [lote.nome, Validators.required],
      quantidade: [lote.quantidade, Validators.required],
      preco: [lote.preco, Validators.required],
      dataInicio: [lote.dataInicio, Validators.required],
      dataFim: [lote.dataFim, Validators.required],
    });
  }

  public changeDateValue(value: Date, index: number, field: string): void {
    this.lotes.value[index][field] = value;
  }

  public onSubmit() {
    if (ValidatorForm.Validate(this.form)) {
      this.spinner.show();

      this.evento =
        this.formAction === FormAction.Post
          ? { ...this.form.value }
          : { id: this.evento.id, ...this.form.value };

      this.eventoService[this.formAction](this.evento)
        .subscribe(
          (evento: Evento) => {
            this.toastr.success(
              `Evento #${evento.id} salvo com sucesso!`,
              'Sucesso!'
            );
            this.router.navigate([`/eventos/detalhe/${evento.id}`]);
          },
          (error: any) => {
            console.error(error);
            this.toastr.error('Ocorreu um erro ao salvar evento', 'Erro!');
          }
        )
        .add(() => this.spinner.hide());
    }
  }

  public onSubmitLotes() {
    if (ValidatorForm.Validate(this.form.controls.lotes)) {
      if (this.eventoId !== null && this.eventoId !== 0) {
        this.spinner.show();

        this.loteService
          .saveLote(this.eventoId, this.form.value.lotes)
          .subscribe(
            (lotes: Lote[]) => {
              this.toastr.success(`Lotes salvos com sucesso!`, 'Sucesso!');
            },
            (error: any) => {
              console.error(error);
              this.toastr.error('Ocorreu um erro ao salvar os lotes', 'Erro!');
            }
          )
          .add(() => this.spinner.hide());
      }
    }
  }

  public cssValidator(campoForm: FormControl | AbstractControl | null): any {
    return {
      'is-invalid': campoForm?.errors && campoForm?.touched,
    };
  }

  public removerLote(index: number, template: TemplateRef<any>): void {
    this.loteAtual.id = this.lotes.get(index + '.id')?.value;
    this.loteAtual.nome = this.lotes.get(index + '.nome')?.value;
    this.loteAtual.index = index;

    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  public confirmDeleteLote(): void {
    this.modalRef?.hide();
    this.spinner.show();
    this.loteService
      .deleteLote(this.eventoId, this.loteAtual.id)
      .subscribe(
        (result: any) => {
          if (result.message === 'Deletado') {
            this.toastr.success(
              `O Lote ${this.loteAtual.nome} foi deletado com sucesso.`,
              'Deletado!'
            );
            this.lotes.removeAt(this.loteAtual.index);
          }
        },
        (error: any) => {
          console.error(error);
          this.toastr.error(
            `Erro ao deletar o Lote ${this.loteAtual.nome}.`,
            'Erro!'
          );
        }
      )
      .add(() => this.spinner.hide());
  }

  public declineDeleteLote(): void {
    this.modalRef?.hide();
  }
}
