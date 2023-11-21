
public class State
{
    public (Chessman chessman, (int x, int y) oldPosition, (int x, int y) newPosition, bool isMoved) movedChessman;
    public (Chessman chessman, (int x, int y) Position) capturedChessman;
    public (int x, int y) EnPassantStatus;
    public (bool wasPromotion, Chessman promotedChessman) PromotionMove;
    public (bool wasCastling, bool isKingSide) CastlingMove;
    public int depth;

    public void SetState((Chessman chessman, (int x, int y) oldPosition, (int x, int y) newPosition, bool isMoved) movedChessman,
                          (Chessman chessman, (int x, int y) Position) capturedChessman,
                          (int x, int y) EnPassantStatus,
                          (bool wasPromotion, Chessman promotedChessman) PromotionMove,
                          (bool wasCastling, bool isKingSide) CastlingMove,
                          int depth)
    {
        this.movedChessman = movedChessman;
        this.capturedChessman = capturedChessman;
        this.EnPassantStatus = EnPassantStatus;
        this.PromotionMove = PromotionMove;
        this.CastlingMove = CastlingMove;
        this.depth = depth;
    }
}
