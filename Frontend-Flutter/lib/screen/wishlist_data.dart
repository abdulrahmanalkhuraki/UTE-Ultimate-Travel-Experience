class WishlistData {
  static final WishlistData instance = WishlistData._();
  WishlistData._();

  final Set<String> _wishlistedIds = {};
  final Map<String, int> _ratings = {};

  void toggleWishlist(String programId) {
    if (_wishlistedIds.contains(programId)) {
      _wishlistedIds.remove(programId);
    } else {
      _wishlistedIds.add(programId);
    }
  }

  bool isWishlisted(String programId) => _wishlistedIds.contains(programId);

  void setRating(String programId, int rating) => _ratings[programId] = rating;

  int getRating(String programId) => _ratings[programId] ?? 0;

  List<Map<String, dynamic>> getWishlistedPrograms(List<Map<String, dynamic>> allPrograms) {
    return allPrograms.where((p) => isWishlisted(p['id'])).toList();
  }
}