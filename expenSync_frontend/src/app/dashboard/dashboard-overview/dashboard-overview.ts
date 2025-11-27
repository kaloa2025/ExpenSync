import { CommonModule } from '@angular/common';
import { Component, input, OnDestroy, OnInit } from '@angular/core';
import { flush } from '@angular/core/testing';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth.service';
import { Subscription } from 'rxjs';
import { AddNewCategoryRequest, Category, ExpenseType, FullTransaction, Icon, ModeOfPayment, Transaction, UpdateCategoryRequest } from '../../../services/dashboard/dashboard.models';
import { DashboardCategory } from '../../../services/dashboard/dashboard.category.service';
import { DashboardProfileService } from '../../../services/dashboard/dashboard.profile.service';
import { EditProfileRequest } from '../../../services/auth/auth.models';
import { DashboardGraphService } from '../../../services/dashboard/dashboard.graph.service';
import { DashboardTransactionService } from '../../../services/dashboard/dashboard.transaction.service';
import { DashboardRecentTransactionsService } from '../../../services/dashboard/dashboard.recent-transactions.service';

@Component({
  selector: 'app-dashboard-overview',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './dashboard-overview.html',
  styleUrl: './dashboard-overview.css',
})
export class DashboardOverview implements OnInit, OnDestroy{

  profileForm : FormGroup;
  transactionForm : FormGroup;
  toastMessage: string = '';
  graphData:{[category:string]:number} = {};

  recentTransactionsByDate: { [date: string]: FullTransaction[] } = {};

  categories : Category[] = [];
  modesOfPayment : ModeOfPayment[] = [];
  expenseTypes : ExpenseType[] = [];
  selectedExpenseType : ExpenseType|null = null;
  filteredCategories : Category[] = [];

  selectedCategoryId : number | null = null;
  availableIcons : Icon[] | null= [];
  selectedIconId : number | null = null;

  showAddCategory = false;
  editCategoryMode = false;
  editProfileMode = false;

  categoryNameInput : string = '';
  showCategoryDeletePopup = false;
  showTransactionDeletePopup = false;
  selectedTransactionId : number | null = null;
  selectedTransaction : Transaction | null = null;

  editCategoryData : any = null;
  isExpenseOpen = false;
  transactionEditMode = false;

  username : string|null = '';
  email : string|null = '';

  monthNames = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
  years: number[] = [];
  month = new Date().getMonth() + 1;
  year = new Date().getFullYear();
  calendarDays: (number | null)[] = [];
  selectedDay : number|null = null;
  selectedFullDate: { day: number, month: number, year: number } | null = null;

  private authSub?: Subscription;

  constructor(private fb:FormBuilder,
    private authService:AuthService,
    private categoryService : DashboardCategory,
    private dashboardProfileService : DashboardProfileService,
    private graphService : DashboardGraphService,
    private transactionService : DashboardTransactionService,
    private recentTransactionService : DashboardRecentTransactionsService,
   )
  {
    this.profileForm=this.fb.group({
      username:[{value:this.username, disabled : true}],
    });
    this.years = this.generateYears();
    this.updateCalendar();
    this.transactionForm = this.fb.group({
      transactionAmount: [null],
      transactionDescription: [''],
      reciverSenderName: [''],
      day: [''],
      month: [''],
      year: [''],
      categoryId: [null],
      expenseTypeId: [null],
      modeOfPaymentId: [null],
    });
    this.getRecentTransactions();
  }

  generateYears(): number[] {
    const current = new Date().getFullYear();
    const range = [];
    for (let y = current - 10; y <= current + 10; y++) {
      range.push(y);
    }
    return range;
  }

  updateCalendar() {
    this.calendarDays = this.generateCalendar(this.year, this.month);
    this.selectedDay = null;
  }

  generateCalendar(year: number, month: number): (number | null)[] {
    const days: (number | null)[] = [];
    const totalDays = new Date(year, month, 0).getDate();
    const firstDay = new Date(year, month - 1, 1).getDay();

    const offset = firstDay === 0 ? 6 : firstDay - 1;
    for (let i = 0; i < offset; i++) days.push(null);
    for (let d = 1; d <= totalDays; d++) days.push(d);
    return days;
  }

  selectDate(day: number) {
    this.selectedDay = day;
    this.selectedFullDate={ day, month: this.month, year: this.year };
    this.transactionForm.patchValue({
      day,
      month: this.month,
      year: this.year
    });
  }

  selectToday() {
    const now = new Date();

    this.year = now.getFullYear();
    this.month = now.getMonth() + 1;
    this.updateCalendar();

    this.selectedDay = now.getDate();
    this.selectedFullDate = {
      day: now.getDate(),
      month: this.month,
      year: this.year
    };
    this.transactionForm.patchValue({
      day: this.selectedDay,
      month: this.month,
      year: this.year
    });
  }

  selectExpenseType(type: ExpenseType) {
    this.selectedExpenseType = type;
    this.transactionForm.patchValue({
      expenseTypeId: type.expenseTypeId
    });
  }

  onManualDateChange() {
    const day = Number(this.transactionForm.get('day')?.value);
    const month = Number(this.transactionForm.get('month')?.value);
    const year = Number(this.transactionForm.get('year')?.value);

    if (year >= 1900 && year <= 2100 && month >= 1 && month <= 12) {
      this.year = year;
      this.month = month;
      this.updateCalendar();
    }

    if (day >= 1 && day <= 31) {
      this.selectedDay = day;
      this.selectedFullDate = { day, month: this.month, year: this.year };
    }
  }

  prepareTransactionRequest() {
    const v = this.transactionForm.value;

    if(v.transactionAmount<=0)
    {
      this.showToast("Please enter an valid Transaction Amount which is greater than 0");
      return;
    }

    if(v.reciverSenderName==null||v.reciverSenderName=="")
    {
      this.showToast("Please select an valid Reviver or Sender");
      return;
    }

    if (!v.day || !v.month || !v.year) {
      this.showToast("Please add a valid date");
      return null;
    }

    if(v.categoryId==null||v.categoryId==0)
    {
      this.showToast("Please select an valid Category");
      return;
    }
    if(v.modeOfPaymentId==null||v.modeOfPaymentId==0)
    {
      this.showToast("Please select an valid Mode of Payment");
      return;
    }

    const isoDate = new Date(v.year, v.month - 1, v.day).toISOString();

    return {
      transactionAmount: Number(v.transactionAmount),
      transactionDescription: v.transactionDescription || null,
      reciverSenderName: v.reciverSenderName ?? "",
      categoryId: Number(v.categoryId),
      expenseTypeId: Number(v.expenseTypeId),
      modeOfPaymentId: Number(v.modeOfPaymentId),
      transactionDate: isoDate
    };
  }

  updateTransaction()
  {
    const request = this.prepareTransactionRequest();
    if (!request) return;

    this.transactionService.editTransaction(request, this.selectedTransaction?.transactionId).subscribe({
      next: res => {
        if(!res || res.success==false)
        {
          this.showToast(res.message);
          return;
        }
        this.showToast("Transaction details updated successfully!");
        this.transactionForm.reset();
        setTimeout(()=>{
          this.loadGraphData();
          this.toggleExpense();
          this.getRecentTransactions();
        },1000);
      },
      error: err => {
        this.showToast("Error adding transaction "+ err?.error?.message);
      }
    });

  }

  submitTransaction() {
    const request = this.prepareTransactionRequest();
    if (!request) return;

    this.transactionService.createTransaction(request).subscribe({
      next: res => {
        if(!res || res.success==false)
        {
          this.showToast(res.message);
          return;
        }
        this.showToast("Transaction added successfully!");
        this.transactionForm.reset();
        setTimeout(()=>{
          this.loadCategories();
          this.loadGraphData();
          this.toggleExpense();
          this.getRecentTransactions();
        },1000);
      },
      error: err => {
        this.showToast("Error adding transaction "+ err?.error?.message);
      }
    });
  }


  ngOnInit(): void {
    this.authSub = this.authService.authState$.subscribe(
      state=>{
        if(!state || !state.claims) return;
        const claims = state.claims;
        this.username=claims.userName??'';
        this.email = claims.email??'';
        this.profileForm.patchValue({username : this.username});
      }
    )
    this.loadExpenseTypes();
    this.loadModesOfPayment();
    this.loadCategories();
    this.loadIcons();
    this.loadGraphData();
  }

  ngOnDestroy(): void {
    if (this.authSub) this.authSub.unsubscribe();
  }

  loadGraphData()
  {
    this.graphService.getGraphData().subscribe({
      next:(res)=>{
        if(res.success && res.data)
        {
          this.graphData = res.data;
        }
        else
        {
          this.showToast(res.message || "Failed to Load Data.");
        }
      },
      error:(err)=>this.showToast(err?.error?.message||"Error Loading Data")
    });
  }

  allGraphRowsZero(): boolean {
    return this.getGraphRows().every(r => r.value === 0);
  }

  getGraphRows(): { name?: string|null, id:number, value: number }[] {
    return this.categories.map(cat => ({
      name: cat.iconUrl,
      id : cat.categoryId,
      value: this.graphData[cat.categoryName] ?? 0
    }));
  }

  getBarWidth(value: number): number {
    const max = Math.max(...this.getGraphRows().map(r => r.value));
    return max ? (value / max) * 100 : 0;
  }

  loadIcons()
  {
    this.categoryService.getAllIcons().subscribe({
      next:(res)=>{
        this.availableIcons=res.icons?? [];
      },
      error : ()=> this.showToast('Failed to load Icons')
    });
  }

  showToast(msg: string) {
    this.toastMessage = msg;

    setTimeout(() => {
      this.toastMessage = '';
    }, 2500);
  }

  loadCategories()
  {
    this.categoryService.getAllCategories().subscribe({
      next:(res)=>{
        if(res.success && res.categories)
        {
          this.categories = res.categories;
          this.filteredCategories = res.categories;
        }
      },
      error:()=> this.showToast('Failed to load categories'),
    });
  }

  loadModesOfPayment()
  {
    this.transactionService.getAllModeOfPayment().subscribe({
      next:(res)=>{
        if(!res || res == null)
        {
          this.showToast('Failed to load Payment modes');
        }
        else
        {
          this.modesOfPayment = res;
        }
      }
    })
  }

    loadExpenseTypes()
  {
    this.transactionService.getAllExpenseType().subscribe({
      next:(res)=>{
        console.log(res);
        if(res == null || !res)
        {
          this.showToast('Failed to load different Expense types');
          console.log('Failed to load different Expense types');
        }
        else
        {
          console.log('Loading different Expense types');
          this.expenseTypes = res;
          console.log(this.expenseTypes);
        }
      }
    })
  }

  onSearch(query : string)
  {
    this.cancelCategoryDelete();
    this.filteredCategories = this.categories.filter((c)=>c.categoryName.toLowerCase().includes(query.toLowerCase()));
  }

  toggleAddCategory()
  {
    this.showAddCategory=!this.showAddCategory;
  }

  editProfileFormActivate()
  {
    this.editProfileMode=true;
    this.profileForm.enable();
  }

  updateProfile()
  {
    this.editProfileMode=false;
    this.profileForm.disable();
    if(this.username!=null && this.email!=null)
    {
      const req : EditProfileRequest = {
        userName : this.profileForm.get('username')?.value,
        email : this.email
      };

      this.dashboardProfileService.updateProfile(req).subscribe({
      next: (res) => {
        if (res.success && res.user) {
          this.showToast(res.message || "Profile updated successfully!");

          this.authService.updateClaims({
            userName : res.user.userName,
          });

        } else {
          this.showToast(res.message || "Failed to update profile");
        }
      },
      error: (err) => {
        this.showToast(err?.error?.message || "Error updating profile");
      }
    });
   }
   else
   {
    this.showToast("Username Can't be empty");
    return;
   }
  }

  scanUploadImage()
  {
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = 'image/*';
    fileInput.style.display = 'none';

    fileInput.onchange=(event:any)=>
    {
      const file = event.target.files[0];
      if(file)
      {
        console.log('selected file : ', file);
        this.showToast("Image Added Successfully!");
      }
    };

    document.body.appendChild(fileInput);
    fileInput.click();
    document.body.removeChild(fileInput);
  }

  toggleExpense(edit?:boolean)
  {
    if((this.transactionForm.get('transactionAmount')?.value)>0 && edit==null)
    {
      this.transactionEditMode = false;
      this.transactionForm = this.fb.group({
        transactionAmount: [null],
        transactionDescription: [''],
        reciverSenderName: [''],
        day: [''],
        month: [''],
        year: [''],
        categoryId: [null],
        expenseTypeId: [null],
        modeOfPaymentId: [null],
      });
      this.selectedTransaction = null;
    }
    else if(edit)
    {
      if(this.isExpenseOpen == false)
      {
        this.isExpenseOpen=!this.isExpenseOpen;
      }
      this.transactionEditMode = true;
      //2025-11-04T18:30:00
      var year = this.selectedTransaction?.transactionDate.substring(0,4);
      var month = this.selectedTransaction?.transactionDate.substring(5,7);
      var day = this.selectedTransaction?.transactionDate.substring(8,10);
      this.transactionForm = this.fb.group({
        transactionAmount: [this.selectedTransaction?.transactionAmount],
        transactionDescription: [this.selectedTransaction?.transactionDescription],
        reciverSenderName: [this.selectedTransaction?.reciverSenderName],
        day: [day],
        month: [month],
        year: [year],
        categoryId: [this.selectedTransaction?.categoryId],
        expenseTypeId: [this.selectedTransaction?.expenseTypeId],
        modeOfPaymentId: [this.selectedTransaction?.modeOfPaymentId],
      });
    }
    else
    {
      this.isExpenseOpen=!this.isExpenseOpen;
    }
  }

  selectCategory(categoryId:number)
  {
    if(this.selectedCategoryId==categoryId)
    {
      this.selectedCategoryId = null;
      this.cancelCategoryDelete();
      return;
    }
    this.selectedCategoryId = categoryId;
    this.categoryService.getCategory(this.selectedCategoryId).subscribe({
      next: (response) => {
        if (!response || !response.category) return;
        this.categoryNameInput = response.category.categoryName;
        this.selectedIconId = response.category.iconId;
        this.editCategoryMode = true;
      },
      error: (err) => {
        this.showToast("Error fetching category:'"+ err);
      }
    });
  }

  confirmDeleteCategory()
  {
    this.showCategoryDeletePopup=true;
  }
  confirmDeleteTransaction(t : Transaction)
  {
    this.showTransactionDeletePopup = true;
    this.selectedTransaction = t;
  }

  deleteTransaction()
  {
    if(this.selectedTransaction!==null)
    {
      this.transactionService.deleteTransaction(this.selectedTransaction.transactionId).subscribe({
        next: (response) => {
          if (!response || !response.success==false)
          {
            this.showToast(response.message || "Couldn't delete specific transaction.");
            return;
          }
        },
        error: (err) => {
          this.showToast("Error deleting transaction: "+err?.error?.message);
          return;
        }
      });
      this.showToast("Transaction Deleted Successfully!");
      this.selectedTransactionId=null;
      this.showTransactionDeletePopup=false;
      setTimeout(()=>{
        this.getRecentTransactions();
      },1000);
    }
  }

  cancelDelete()
  {
    this.showCategoryDeletePopup=false;
    this.showTransactionDeletePopup=false;
  }

  cancelCategoryDelete()
  {
    this.categoryNameInput='';
    this.selectedCategoryId = null;
    this.selectedIconId = null;
    this.showAddCategory = false;
    this.editCategoryMode=false;
  }

  deleteCategory()
  {
    if(this.selectedCategoryId!==null)
    {
      this.categoryService.deleteCategory(this.selectedCategoryId).subscribe({
        next: (response) => {
          if (!response || !response.success==false)
          {
            this.showToast(response.message || "Couldn't find any such Category.");
            this.cancelCategoryDelete();
            return;
          }
        },
        error: (err) => {
          this.cancelCategoryDelete();
          this.showToast("Error deleting category: "+err?.error?.message);
        }
      });

      this.showToast("Category Deleted Successfully!");
      this.selectedCategoryId=null;
      this.showCategoryDeletePopup=false;
      setTimeout(()=>{
        this.loadCategories();
      },1000);
    }
  }

  displayEditCategorySection()
  {
    if(!this.selectedCategoryId)
    {
      this.showToast("Please select an Category to Edit.");
      return;
    }
    this.editCategoryMode = true;
    this.showAddCategory = true;
  }

  updateCategory(editCategoryData :any)
  {
    if (!this.selectedCategoryId) {
      this.showToast("No category selected");
      return;
    }
   if (!this.categoryNameInput.trim()) {
      this.showToast("Category name cannot be empty");
      return;
    }
    if(!this.selectedIconId)
    {
      this.showToast("Please select an Icon");
      return;
    }
    if (this.editCategoryMode && this.selectedCategoryId !== null)
    {
      const request : UpdateCategoryRequest =
      {
        categoryName : this.categoryNameInput,
        iconId : this.selectedIconId
      };

      this.categoryService.updateCategory(this.selectedCategoryId, request).subscribe(
      {
        next:(res)=>{
          if(res.success){
            this.showToast("Category updated Scuccessfully");
            setTimeout(()=>{
              this.loadCategories();
            },1000);
          }
          else
          {
            this.showToast(res.message||"Failed to update Category");
          }
          this.showAddCategory = false;
          this.editCategoryMode = false;
          this.categoryNameInput = '';
          this.selectedCategoryId = null;
          this.selectedIconId = null;
        },
        error: (err) => {
          const errorMsg = err?.error?.message || "Error Updating Category";
          this.showToast(errorMsg);
          this.cancelCategoryDelete();
        }
      });
    }
  }

  saveCategory(editedCategory :any)
  {
    if (!this.categoryNameInput.trim()) {
      this.showToast("Category name cannot be empty");
      return;
    }
    if(!this.selectedIconId)
    {
      this.showToast("Please select an Icon");
      return;
    }
    const req : AddNewCategoryRequest = {
      categoryName : this.categoryNameInput,
      iconId : this.selectedIconId
    };

    this.categoryService.addCategory(req).subscribe({
      next:(res) => {
        if(res.success)
        {
          this.showToast("Category added Successfully");
          setTimeout(()=>{
            this.loadCategories();
          },1000);
        }
        else
        {
          this.showToast(res.message||"Failed to add Category");
        }
        this.showAddCategory = false;
        this.showAddCategory = false;
        this.editCategoryMode = false;
        this.categoryNameInput = '';
        this.selectedCategoryId = null;
        this.selectedIconId = null;
      },
      error : (err)=> this.showToast("Error Adding Category "+ err?.error?.message)
    });
  }

  getRecentTransactions()
  {
    this.recentTransactionService.getRecentTransactions().subscribe({
      next:(res)=>
      {
        if(res.success && res.transactionsByDate)
        {
          this.recentTransactionsByDate = res.transactionsByDate;
        }
        else {
        this.showToast(res.message ?? "Failed to load recent transactions");
        }
      },
      error: err => {
        this.showToast(err?.error?.message ?? "Error loading recent transactions");
      }
    });
  }

  findCategoryName(categoryId : number)
  {
    return this.categories.find(c=>c.categoryId===categoryId)?.categoryName;
  }

  findCategoryIcon(categoryId : number)
  {
    return this.categories.find(c=>c.categoryId===categoryId)?.iconUrl;
  }

  isAtBottom = false;

  onScroll(event: any) {
    const div = event.target;

    const atBottom = div.scrollTop + div.clientHeight >= div.scrollHeight - 1;

    this.isAtBottom = atBottom;
  }

  getReport()
  {
    this.transactionService.getReport().subscribe({
      next:(res)=>{
        if(res.success==true)
        {
          this.showToast("Report generated and sent on your email");
        }
        else
        {
          this.showToast("Error generating report. Try again later.");
        }
      },
      error:err=>{this.showToast(err?.error.message)}
    })
  }

  editTransaction(t:Transaction)
  {
    this.selectedTransaction = t;
    this.toggleExpense(true);
  }

  logout()
  {
    this.authService.logout(true);
    this.showToast("Logout sucessfull.");
  }
}
