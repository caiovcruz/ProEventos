import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorForm } from '@app/helpers/ValidatorForm';
import { UserLogin } from '@app/models/identity/UserLogin';
import { AccountService } from '@app/services/account.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  user = {} as UserLogin;
  form!: FormGroup;

  get f(): any {
    return this.form.controls;
  }

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnInit(): void {
    this.validation();
  }

  public validation(): void {
    this.form = this.fb.group({
      userName: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  public onSubmit(): void {
    if (ValidatorForm.Validate(this.form)) {
      this.spinner.show();

      this.user = { ...this.form.value };

      this.accountService
        .login(this.user)
        .subscribe(
          () => {
            this.router.navigateByUrl('/dashboard');
          },
          (error: any) => {
            if (error.status == 401) {
              this.toastr.error('Usuário ou senha inválido');
            } else {
              console.error(error);
            }
          }
        )
        .add(() => this.spinner.hide());
    }
  }

  public cssValidator(campoForm: FormControl | AbstractControl | null): any {
    return {
      'is-invalid': campoForm?.errors && campoForm?.touched,
    };
  }
}
