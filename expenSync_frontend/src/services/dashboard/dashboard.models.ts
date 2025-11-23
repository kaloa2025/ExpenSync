export interface Category
{
  categoryId : number;
  userId? : number;
  categoryName : string;
  isDefault : number;
  iconId : number;
  iconUrl : string | null;
}

export interface Icon
{
  iconId : number;
  iconImageUrl? : string | null;
}

export interface FetchIconsResponse
{
  success :boolean;
  icons : Icon[] | null;
  message? : string;
  errors? :string[];
}

export interface FetchCategoriesResponse {
  success: boolean;
  categories: Category[] | null;
  message?: string;
  errors?: string[];
}

export interface FetchCategoryResponse {
  success: boolean;
  category: Category | null;
  message?: string;
  errors?: string[];
}

export interface AddNewCategoryRequest {
  categoryName: string;
  iconId: number;
}

export interface AddNewCategoryResponse {
  success: boolean;
  category?: Category;
  message?: string;
  errors?: string[];
}

export interface UpdateCategoryRequest {
  categoryName: string;
  iconId: number;
}

export interface UpdateCategoryResponse {
  success: boolean;
  category?: Category;
  message?: string;
  errors?: string[];
}

export interface DeleteCategoryResponse
{
  success: boolean;
  message?: string;
  errors?:string[];
}

export interface EditProfileResponse
{
  success:boolean;
  message?:string;
  user:{
    userId : number;
    userName : string;
    email : string;
    role : string;
  }
  errors?:string[];
}

export interface GraphDataResponse
{
  success: boolean;
  message?:string;
  errors?:string[];
  data?:{[category:string]:number};
}
