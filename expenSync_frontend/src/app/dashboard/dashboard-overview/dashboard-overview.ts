import { CommonModule, NgIf } from '@angular/common';
import { Component, input, OnDestroy, OnInit } from '@angular/core';
import { flush } from '@angular/core/testing';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth.service';
import { Subscription } from 'rxjs';
import { AddNewCategoryRequest, Category, Icon, UpdateCategoryRequest } from '../../../services/dashboard/dashboard.models';
import { DashboardCategory } from '../../../services/dashboard/dashboard.category.service';
import { DashboardProfileService } from '../../../services/dashboard/dashboard.profile.service';
import { EditProfileRequest } from '../../../services/auth/auth.models';
import { DashboardGraphService } from '../../../services/dashboard/dashboard.graph.service';

@Component({
  selector: 'app-dashboard-overview',
  imports: [CommonModule, NgIf, ReactiveFormsModule, FormsModule],
  templateUrl: './dashboard-overview.html',
  styleUrl: './dashboard-overview.css',
})
export class DashboardOverview implements OnInit, OnDestroy{

  profileForm : FormGroup;
  toastMessage: string = '';
  graphData:{[category:string]:number} = {};

  categories : Category[] = [];
  filteredCategories : Category[] = [];

  selectedCategoryId : number | null = null;
  availableIcons : Icon[] | null= [];
  selectedIconId : number | null = null;

  showAddCategory = false;
  editCategoryMode = false;
  editProfileMode = false;

  categoryNameInput : string = '';
  showDeletePopup = false;

  editCategoryData : any = null;
  isExpenseOpen = false;

  username : string|null = '';
  email : string|null = '';

  private authSub?: Subscription;

  constructor(private fb:FormBuilder, private authService:AuthService, private categoryService : DashboardCategory, private dashboardProfileService : DashboardProfileService, private graphService : DashboardGraphService )
  {
    this.profileForm=this.fb.group({
      username:[{value:this.username, disabled : true}],
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

  getGraphRows(): { name?: string|null, value: number }[] {
    return this.categories.map(cat => ({
      name: cat.iconUrl,
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

  toggleExpense()
  {
    this.isExpenseOpen=!this.isExpenseOpen;
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
      },
      error: (err) => {
        this.showToast("Error fetching category:'"+ err);
      }
    });
  }

  confirmDeleteCategory()
  {
    this.showDeletePopup=true;
  }

  cancelDelete()
  {
    this.showDeletePopup=false;
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
            return;
          }
        },
        error: (err) => {
          this.showToast("Error deleting category: "+err?.error?.message);
        }
      });
      this.loadCategories();
      this.showToast("Category Deleted Successfully!");
      this.selectedCategoryId=null;
      this.showDeletePopup=false;
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
            this.loadCategories();
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
          this.loadCategories();
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
      error : ()=> this.showToast("Error Adding Category")
    });
  }

  logout()
  {
    this.authService.logout(true);
    this.showToast("Logout sucessfull.");
  }
}
