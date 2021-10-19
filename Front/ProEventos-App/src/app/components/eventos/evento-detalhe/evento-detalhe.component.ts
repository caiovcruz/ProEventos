import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { FormAction } from '@app/helpers/formAction.enum';
import { ValidatorForm } from '@app/helpers/ValidatorForm';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/services/evento.service';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss'],
})
export class EventoDetalheComponent implements OnInit {
  evento = {} as Evento;
  form!: FormGroup;
  formAction = FormAction.Post;

  get f(): any {
    return this.form.controls;
  }

  get bsConfig(): any {
    return {
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY hh:mm A',
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
    private eventoService: EventoService
  ) {
    this.localeService.use('pt-br');
  }

  public carregarEvento(): void {
    const eventoIdParam = this.activatedRoute.snapshot.paramMap.get('id');

    if (eventoIdParam !== null) {
      this.spinner.show();

      this.formAction = FormAction.Put;
      this.eventoService.getEventoById(+eventoIdParam).subscribe(
        (evento: Evento) => {
          this.evento = { ...evento };
          this.form.patchValue(this.evento);
        },
        (error: any) => {
          console.error(error);
          this.toastr.error('Erro ao carregar o Evento.', 'Erro!');
        }
      ).add(() => this.spinner.hide());
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
    });
  }

  public onSubmit() {
    if (ValidatorForm.Validate(this.form)) {
      this.spinner.show();

      this.evento = (this.formAction === FormAction.Post)
        ? {...this.form.value}
        : {id: this.evento.id, ...this.form.value};

      this.eventoService[this.formAction](this.evento).subscribe(
        (evento: Evento) => {
          this.toastr.success(`Evento #${evento.id} salvo com sucesso!`, 'Sucesso!');
          this.router.navigate(['/eventos/lista'])
        },
        (error: any) => {
          console.error(error);
          this.toastr.error('Ocorreu um erro ao salvar evento', 'Erro!')
        }
      ).add(() => this.spinner.hide());
    }
  }

  public cssValidator(campoForm: FormControl): any {
    return {
      'is-invalid': campoForm.errors && campoForm.touched,
    };
  }
}
