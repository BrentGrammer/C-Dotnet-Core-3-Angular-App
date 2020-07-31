// interface for what is returned in the response headers from getusers call to api
export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

/**
 * this class is used to store the users returned from the api call to get items(i.e. users and messages) and also the pagination information
 * class made with generic so it can be used with both users and items
 * This class is used in the user.service for example in the getUsers()
 */
export class PaginatedResult<T> {
  result: T; // the items returned from api
  pagination: Pagination; // pagination info
}
