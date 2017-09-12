# FontAwesomeEnum
Creates enums and resources C# classes to be able to use the Font-Awesome icons set (currently version 4.7, available here: http://fortawesome.github.io/Font-Awesome/) in .NET a WPF applications.

The generated FontAwesomeEnum.cs file looks like this:

    namespace FontAwesome
    {
      public enum FontAwesomeEnum
      {
        fa_500px = 0xf26e,

        fa_address_book = 0xf2b9,

        fa_address_book_o = 0xf2ba,

        fa_address_card = 0xf2bb,
        
        .... etc ...
      }
    }

The generated FontAwesomeConstants.cs file looks like this:

  namespace FontAwesome
  {
    public static class FontAwesomeResource
    {
      public const char fa_500px = '\uf26e';

      public const char fa_address_book = '\uf2b9';

      public const char fa_address_book_o = '\uf2ba';

      public const char fa_address_card = '\uf2bb';
      
    .... etc ...
    
    }
  }