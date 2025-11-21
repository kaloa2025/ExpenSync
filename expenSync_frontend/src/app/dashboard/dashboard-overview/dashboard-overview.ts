import { CommonModule, NgIf } from '@angular/common';
import { Component, input, OnDestroy, OnInit } from '@angular/core';
import { flush } from '@angular/core/testing';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth.service';
import { Subscription } from 'rxjs';
import { Category, Icon } from '../../../services/dashboard/dashboard.models';
import { DashboardCategory } from '../../../services/dashboard/dashboard.category';

@Component({
  selector: 'app-dashboard-overview',
  imports: [CommonModule, NgIf, ReactiveFormsModule],
  templateUrl: './dashboard-overview.html',
  styleUrl: './dashboard-overview.css',
})
export class DashboardOverview implements OnInit, OnDestroy{

  profileForm : FormGroup;

  categories : Category[] = [];
  filteredCategories : Category[] = [];
  selectedCategoryId : number | null = null;
  availableIcons : Icon[] | null= [];
  selectedIconId : number | null = null;

  showAddCategory = false;
  editMode = false;

  editCategoryData : any = null;
  isExpenseOpen = false;
  showDeletePopup = false;

  username : string|null = '';
  email : string|null = '';

  private authSub?: Subscription;

  constructor(private fb:FormBuilder, private authService:AuthService, private categoryService : DashboardCategory)
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
  }

  ngOnDestroy(): void {
    if (this.authSub) this.authSub.unsubscribe();
  }

  loadIcons()
  {
    this.categoryService.getAllIcons().subscribe(res=>this.availableIcons=res.icons);
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
      error:()=> alert('Failed to load categories'),
    });
  }

  onSearch(query : string)
  {
    this.filteredCategories = this.categories.filter((c)=>c.categoryName.toLowerCase().includes(query.toLowerCase()));
  }

  toggleAddCategory()
  {
    this.showAddCategory=!this.showAddCategory;
  }

  editFormActivate()
  {
    this.editMode=true;
    this.profileForm.enable();
  }

  updateProfile()
  {
    this.editMode=false;
    this.profileForm.disable();
    alert("Profile Updated! " + this.profileForm.value);
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
        alert("Image Added Successfully!");
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
    this.selectedCategoryId = categoryId;
  }

  clearSelected()
  {
    this.selectedCategoryId = null;
  }

  confirmDeleteCategory()
  {
    this.showDeletePopup=true;
  }

  cancelDelete()
  {
    this.showDeletePopup=false;
  }

  deleteCategory()
  {
    if(this.selectedCategoryId!==null)
    {
      alert("Category Deleted Successfully!");
      this.selectedCategoryId=null;
      this.showDeletePopup=false;
    }
  }

  editCategory()
  {
    if (this.selectedCategoryId !== null) {
      this.editMode = true;
      this.showAddCategory = true;
      if(!this.selectedIconId)
      {
        alert("Please select an Icon");
        return;
      }
    }
  }

  saveCategory(editedCategory :any)
  {
    if(!this.selectedIconId)
    {
      alert("Please select an Icon");
      return;
    }
    if (this.editMode && this.selectedCategoryId !== null)
    {
      this.editMode = false;
      this.showAddCategory = false;
      this.editCategoryData = null;
      this.selectedCategoryId = null;
    } else {
      const payload = {
        categoryName: this.selectCategory.name,
        iconId: this.selectedIconId
      };

      this.categoryService.addCategory(payload).subscribe({
        next: () => {
            this.loadCategories();
            this.showAddCategory = false;
        }
      });
    }
  }

  logout()
  {
    this.authService.logout(true);
  }
}
