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
