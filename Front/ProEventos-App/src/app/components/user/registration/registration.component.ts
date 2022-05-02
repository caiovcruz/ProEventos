import { Component, OnInit } from '@angular/core';
import {
  AbstractControlOptions,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { ValidatorForm } from '@app/helpers/ValidatorForm';
import { User } from '@app/models/identity/User';
import { AccountService } from '@app/services/account.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss'],
})
export class RegistrationComponent implements OnInit {
  user = {} as User;
  form!: FormGroup;

  get f(): any {
    return this.form.controls;
  }

  constructor(
    private fb: FormBuilder,
    private spinner: NgxSpinnerService,
    private accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.validation();
  }

  public validation(): void {
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmarPassword'),
    };

    this.form = this.fb.group(
      {
        primeiroNome: ['', Validators.required],
        ultimoNome: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        userName: ['', [Validators.required, Validators.minLength(3)]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmarPassword: ['', Validators.required],
        termosUso: ['', Validators.requiredTrue],
      },
      formOptions
    );
  }

  public onSubmit() {
    if (ValidatorForm.Validate(this.form)) {
      this.spinner.show();

      this.user = { ...this.form.value };

      this.accountService.register(this.user).subscribe(
        () => this.router.navigateByUrl('/dashboard'),
        (error: any) => this.toastr.error(error.error)
      )
      .add(() => this.spinner.hide());
    }
  }
}
